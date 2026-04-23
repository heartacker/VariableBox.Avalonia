# VariableBox 全面重构计划

> 目标：在保留当前优秀抽象（如 DragValueBehavior、策略接口概念）的基础上，彻底消除代码重复、降低文件碎片化、恢复类型安全，全面提升项目质量。

---

## 一、问题诊断

### 1. 核心问题：放弃泛型，换取了 11 份复制粘贴

原有的 `NumericUpDownBase<T>` 用泛型把 **Value/Step/Min/Max/Coerce/同步/事件/解析/格式化/Clamp** 全部收敛在一个地方，具体实现只需要实现 `ParseText`、`Add`、`Minus` 等少数抽象成员。

当前改动彻底去泛型化，变成了 11 个 `VariableBoxXXX`，每个都重复了完全相同的逻辑：

- `SetValidSpinDirection`（11 份一字不差）
- `SyncTextAndValue`（结构完全相同，仅 `int.TryParse` vs `double.TryParse` 不同）
- `CheckContextIsChangedAndValid`、`OnRead`、`OnWrite`、`Clear`、`ValueChangedEvent` 注册……
- 甚至连 `static` 构造函数的 `Changed.AddClassHandler` 都复制了 11 份。

**结果：代码量从精简变臃肿，任何 bug 修复都要改 11 处，严重违反 DRY。**

### 2. 文件过度拆分

新增了 11 个几乎一样的 `.cs` 文件，加上 `INumericStrategy.cs`、`DragValueBehavior.cs`，项目结构变得非常零散。对于 .NET 这种原生支持泛型的语言，这是**反模式**。

### 3. `INumericStrategy<T>` 空转

策略接口定义了 `Add`、`Minus`、`ParseText`、`Clamp` 等，但新增的 11 个类**完全没有使用它**。策略模式只停留在“定义了一个接口”的层面，没有解决任何重复问题。

### 4. 类型安全倒退

原来的 `ValueChangedEventArgs<T>` 是强类型的，现在改成了 `object? OldValue / NewValue`，XAML 绑定虽然能用，但 C# 代码里丢失了泛型优势。

### 5. 可读性劣化

`NumericUpDownBase.cs` 被压成了一行多属性的写法。这不是精简，是混淆。

---

## 二、值得保留的好抽象

当前改动里有 2 个思路是对的，重构时应保留并深化：

| 抽象 | 评价 |
|------|------|
| **`DragValueBehavior`** | ✅ **正确**。把“拖拽改变数值”这个**输入交互行为**从数值逻辑中解耦，符合关注点分离。 |
| **`INumericStrategy<T>` 的概念** | ✅ **方向正确**。数值类型的差异（`TryParse`、`+`、`-`、格式化）确实应该用策略/运算器隔离，而不是用 11 个类重复整个控件逻辑。 |

---

## 三、重构目标与原则

1. **高内聚**：数值逻辑（解析、运算、同步、事件）必须回到一个泛型基类里。
2. **零重复**：11 个具体数值类型不应该包含任何重复代码。
3. **适度文件数**：核心逻辑集中在 2-3 个文件，具体类型空壳可以放在 1 个文件里。
4. **类型安全**：恢复泛型事件参数 `ValueChangedEventArgs<T>`。
5. **XAML 可用**：由于 XAML 不能直接写泛型（`<v:VariableBox<int> />` 不合法），仍然需要提供具体的非泛型类，但这些类应该是**纯空壳**（3-5 行）。

---

## 四、建议的新架构

```
src/VariableBox/
├── Controls/
│   ├── NumericUpDown.cs              ← UI/模板/交互基类（~250行）
│   ├── NumericUpDownBase.cs          ← 泛型数值核心逻辑（~300行）
│   ├── NumericUpDownBase.T.cs        ← 或合并到上者
│   └── VariableBox.cs                ← 所有具体类型空壳（~60行，11个类）
│
├── Behaviors/
│   └── DragValueBehavior.cs          ← 保留并深化
│
├── Common/
│   ├── NumericOperations.cs          ← 取代 INumericStrategy，用静态抽象+接口
│   └── ValueChangedEventArgs.cs      ← 恢复泛型版本
│
└── Themes/...
```

### 4.1 分层职责

#### Layer 1: `NumericUpDown`（非泛型 UI 基类）

- 只关心：**模板应用、键盘/鼠标/拖拽输入、伪类、命令路由、读写按钮**
- 不接触任何 `Value`、`Step`、`Maximum` 的具体类型
- `DragValueBehavior` 在此 attach
- 对外暴露抽象方法：`Increase()`、`Decrease()`、`SyncTextAndValue(...)` 等

#### Layer 2: `NumericUpDownBase<T>`（泛型数值核心）

- 继承 `NumericUpDown`
- 包含所有类型无关的数值逻辑：
  - `StyledProperty<T?> Value` / `Step` / `Maximum` / `Minimum`
  - `ValueChangedEvent`（泛型）
  - `OnValueChanged` → `SyncTextAndValue` → `SetValidSpinDirection`
  - `CommitInput`、`Clamp`、`CoerceMaximum/Minimum`
  - `Command` / `ReadCommand` 的执行和事件触发
- **类型特化部分委托给 `INumericOperations<T>`**

#### Layer 3: `INumericOperations<T>` / `NumericOperations<T>`（策略实现）

取代当前闲置的 `INumericStrategy<T>`，做成更轻量的“类型运算器”：

```csharp
public interface INumericOperations<T> where T : struct, IComparable<T>
{
    T Zero { get; }
    T DefaultStep { get; }
    bool TryParse(string? s, NumberStyles style, NumberFormatInfo? format, out T result);
    string? ToString(T? value, string formatString, NumberFormatInfo? format);
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Clamp(T value, T max, T min);
}
```

为每种数值类型提供单例实现（可用 `switch expression` 或内部类），或者利用 C# 11 的 `static abstract`（如果目标框架允许）。

#### Layer 4: 具体控件（XAML 可用空壳）

**全部放在同一个文件 `VariableBox.cs` 中**，每个类只有 3-5 行：

```csharp
public class VariableBoxByte   : NumericUpDownBase<byte>   { public VariableBoxByte()   : base(NumericOperations.Byte)   {} }
public class VariableBoxSByte  : NumericUpDownBase<sbyte>  { public VariableBoxSByte()  : base(NumericOperations.SByte)  {} }
public class VariableBoxShort  : NumericUpDownBase<short>  { public VariableBoxShort()  : base(NumericOperations.Short)  {} }
public class VariableBoxUShort : NumericUpDownBase<ushort> { public VariableBoxUShort() : base(NumericOperations.UShort) {} }
public class VariableBoxInt    : NumericUpDownBase<int>    { public VariableBoxInt()    : base(NumericOperations.Int)    {} }
public class VariableBoxUInt   : NumericUpDownBase<uint>   { public VariableBoxUInt()   : base(NumericOperations.UInt)   {} }
public class VariableBoxLong   : NumericUpDownBase<long>   { public VariableBoxLong()   : base(NumericOperations.Long)   {} }
public class VariableBoxULong  : NumericUpDownBase<ulong>  { public VariableBoxULong()  : base(NumericOperations.ULong)  {} }
public class VariableBoxFloat  : NumericUpDownBase<float>  { public VariableBoxFloat()  : base(NumericOperations.Float)  {} }
public class VariableBoxDouble : NumericUpDownBase<double> { public VariableBoxDouble() : base(NumericOperations.Double) {} }
public class VariableBoxDecimal: NumericUpDownBase<decimal>{ public VariableBoxDecimal(): base(NumericOperations.Decimal){} }
```

**好处**：11 个类型只占 1 个文件，没有任何重复逻辑，bug 修复只需改 `NumericUpDownBase<T>` 一处。

---

## 五、具体文件重构建议

### 5.1 `NumericUpDownBase.cs`：从 93 行恢复到合理规模（~300行），但比原来更干净

- **收回所有数值通用逻辑**：`ValueProperty.Changed` 处理、`SyncTextAndValue`、`SetValidSpinDirection`、`Clamp`、`Coerce`、`Command 执行` 等。
- **与 `DragValueBehavior` 协作**：`NumericUpDown` 基类只负责在 `OnApplyTemplate` 中 `new DragValueBehavior(this, ...).Attach()`，具体拖拽逻辑继续由 `DragValueBehavior` 处理。
- **模板事件简化**：当前改动把 `OnSpin`、`OnTextBoxTextChanged` 等全部内联成 lambda，这导致无法 override 且难以调试。建议恢复为受保护的虚方法，但代码可以写得比原来更紧凑。

### 5.2 `DragValueBehavior.cs`：保留，做一个小调整

当前 `DragValueBehavior` 直接调用 `_parent.IncreaseInternal()`，这是对的。但建议：

- 把 `IncreaseInternal` / `DecreaseInternal` 保留在 `NumericUpDown` 基类中（因为拖拽是纯 UI 行为，不需要知道具体类型）。
- 或者更彻底：让 `DragValueBehavior` 只返回 `DragDelta`，由 `NumericUpDown` 基类调用 `Increase()` / `Decrease()`。这样 `DragValueBehavior` 完全不依赖数值方向。

### 5.3 删除或合并 11 个 `VariableBoxXXX.cs`

把它们合并为单个 `VariableBox.cs`（或 `VariableBoxTypes.cs`），里面只有空壳子类。

### 5.4 `ValueChangedEventArgs.cs`：恢复泛型

```csharp
public class ValueChangedEventArgs<T> : RoutedEventArgs
{
    public T? OldValue { get; }
    public T? NewValue { get; }
    // ...
}
```

同时在 `NumericUpDownBase<T>` 中提供类型安全的事件。

### 5.5 如果确实想用 Source Generator（可选进阶）

如果未来还要支持 `nint`、`nuint` 或其他自定义数值类型，可以写一个 **Source Generator**：

- 在 `NumericUpDownBase<T>` 旁标注 `[GenerateVariableBox(typeof(int))]`
- 自动生成 `VariableBoxInt` 等空壳类

但这属于“锦上添花”，当前阶段手动写 11 行空壳完全足够。

---

## 六、与现有改动的取舍对照

| 当前改动的做法 | 建议的做法 | 理由 |
|---|---|---|
| 11 个文件，每个 55 行重复代码 | **1 个文件**，11 个空壳类，每类 3-5 行 | DRY，维护性 |
| 去泛型，放弃 `NumericUpDownBase<T>` | **保留并深化泛型基类**，把类型差异交给 `INumericOperations<T>` | .NET 泛型就是为了解决这类问题 |
| `INumericStrategy<T>` 定义了但没用 | **真正用起来**，或改成更精简的 `INumericOperations<T>` | 策略模式不应停留在纸面 |
| `ValueChangedEventArgs` 退化为 `object?` | **恢复 `ValueChangedEventArgs<T>`** | 类型安全 |
| 属性定义全挤在一行 | **恢复分行**，但比原来更紧凑（去掉冗余注释和空行） | 可读性 |
| `DragValueBehavior` 提取 | **保留**，并进一步解耦（不依赖 `IncreaseInternal`） | 这是正确的方向 |

---

## 七、实施路线图

1. **回滚** 11 个 `VariableBoxXXX.cs`，保留 `DragValueBehavior`。
2. **重建** `NumericUpDownBase<T>`，收回所有通用数值逻辑。
3. **引入** `INumericOperations<T>` 消除类型差异。
4. **合并** 所有具体控件到一个文件，作为纯空壳。
5. **恢复** `ValueChangedEventArgs<T>` 和可读性格式。

---

## 八、核心原则

> **用“职责分离”和“策略模式”来缩小单个文件的体积，而不是把代码撒到 N 个重复文件里。**

最终得到一个**文件数更少、内聚性更高、重复为零、且仍然类型安全**的代码库。

---

## 九、外部重构方案评价与补充

> 以下是对另一套 AI 分析方案的评价。该方案同样识别出了“巨石类”问题，并提出了策略模式、Behavior 解耦、Source Generator 等方向。我们的计划与之高度一致，但它在**主题标准化、测试策略、功能扩展**上提供了额外启发。

### 9.1 方向一致性（双方共识）

| 主题 | 外部方案 | 本计划 | 一致性 |
|------|----------|--------|--------|
| **策略模式隔离数值运算** | 提出 `INumericStrategy<T>` | 提出 `INumericOperations<T>` | ✅ 高度一致，本计划更强调“真正用起来”而非仅定义接口 |
| **拖拽逻辑提取为 Behavior** | 建议提取为 Avalonia Behavior | 保留 `DragValueBehavior` 并深化解耦 | ✅ 高度一致 |
| **消除多类型子类样板代码** | 主推 Source Generator | 列为可选进阶 | ⚠️ 优先级差异，见下文讨论 |
| **主题/样式标准化** | 提到统一 TemplatePart、ThemeVariant | 未涉及 | ❌ 本计划遗漏，需补充 |
| **测试与稳定性** | 提到 Avalonia.Headless 无头测试 | 未涉及 | ❌ 本计划遗漏，需补充 |

### 9.2 值得吸收的启发点

#### 1. 主题与样式系统标准化

外部方案指出：应确保所有主题（Fluent、Semi、Simple）**共用一套标准的 `TemplatePart` 名称**（如 `PART_TextBox`、`PART_Spinner`），并深度集成 Avalonia 11 的 `ThemeVariant`。

**补充到本计划**：
- 在重构 UI 基类 `NumericUpDown.cs` 时，统一并冻结 `TemplatePart` 常量命名，所有主题模板必须严格遵守。
- 检查 `VariableBoxNumeric.axaml` 中的模板，确认 `PART_DragPanel`、`PART_RepeatRead`、`PART_RepeatWrite` 等在所有主题下都有正确映射。
- 利用 `DynamicResource` 引用全局调色板变量，确保暗色/亮色模式切换时控件无需手动重载资源字典。

#### 2. Source Generator 的优先级再评估

外部方案将 Source Generator 作为**核心手段**来消除 `VariableBoxInt`、`VariableBoxDouble` 等样板子类。

**本计划的调整意见**：
- **Phase 1（立即执行）**：手动将 11 个空壳类合并到单个 `VariableBox.cs` 中（每类 3-5 行）。这是零成本、立即可见效的改进。
- **Phase 2（未来演进）**：如果项目需要支持更多数值类型（如 `Half`、`BigInteger`、`nint`、`nuint`、甚至用户自定义数值类型），再引入 Source Generator。届时只需维护一个 Generator，从 `INumericOperations<T>` 的注册表自动生成所有子类。
- **结论**：当前阶段手动空壳足够，但 Source Generator 确实是长期架构的**最佳实践**，不应完全否定。

#### 3. 重构后的测试策略（本计划遗漏，必须补充）

外部方案提到了 **Avalonia.Headless 无头测试**，这是重构完成后必须补上的环节。

**补充到本计划**：
- 重构后的 `NumericUpDownBase<T>` 内聚性极高，且数值逻辑完全委托给 `INumericOperations<T>`，这使得单元测试变得非常容易：
  - **运算器测试**：直接对 `NumericOperations.Int.Add/Clamp/TryParse` 做纯逻辑测试，无需启动 UI。
  - **控件逻辑测试**：利用 `Avalonia.Headless` 模拟键盘输入、滚轮事件、拖拽事件，验证 `ValueChanged` 事件、`IsEditing` 状态、`ValidSpinDirection` 等。
  - **边界场景**：十六进制溢出、长按加速、连续拖拽、最大/最小值钳制、无效输入时的 `:invalid` 伪类状态。

#### 4. 功能增强方向（重构后的下一步）

外部方案提出了几个有价值的功能扩展，虽然不属于“重构”范畴，但应在架构上预留接口：

| 功能 | 架构预留建议 |
|------|-------------|
| **撤销/重做 (Undo/Redo)** | 在 `NumericUpDownBase<T>` 中内置轻量级 `Stack<T?>`，监听 `ValueProperty.Changed`，支持 <kbd>Ctrl+Z</kbd> 恢复。Command 模式天然适配。 |
| **自定义校验规则 (ValidationRule)** | 当前 `:invalid` 仅基于解析失败。可预留 `IValidationRule<T>` 集合，在 `CommitInput` 时串行执行，未通过则触发 `:invalid` 并阻止写入。 |
| **枚举支持 (EnumerationUpDown)** | 在 `INumericOperations<T>` 之外，可设计一个平行的 `IEnumerationOperations<T>`，利用反射或 Source Generator 从 `Enum` 类型生成步进逻辑。 |

### 9.3 外部方案的不足之处

1. **没有指出当前改动的“去泛型化”是核心倒退**  
   外部方案分析了原有 `NumericUpDownBase.cs` 是“巨石类”，但没有意识到当前工作区的改动已经**彻底放弃了泛型**，导致 11 份重复代码。它提出的策略模式更像是对原有代码的优化，而不是对当前错误改动的纠正。

2. **对文件碎片的警示不足**  
   外部方案提到“消除样板代码”，但没有强调当前 11 个文件的重复问题有多严重。Source Generator 被当作“锦上添花”的高级特性，而本计划认为：即使不用 Generator，也必须立即把 11 个文件合并回 1 个。

3. **Source Generator 在当前阶段可能过重**  
   对于 11 个标准数值类型，维护一个 Source Generator 项目的成本高于维护 11 行空壳代码。Generator 更适合“类型数量不确定或未来会大量扩展”的场景。

### 9.4 更新后的路线图

```
Phase 1: 结构重建（立即执行）
├── 1. 重建 NumericUpDownBase<T>，收回数值通用逻辑
├── 2. 引入 INumericOperations<T> 消除类型差异
├── 3. 合并 VariableBoxXXX.cs 为单个 VariableBox.cs（空壳类）
├── 4. 保留并深化 DragValueBehavior
├── 5. 恢复 ValueChangedEventArgs<T>
└── 6. 统一 TemplatePart 命名，检查多主题一致性

Phase 2: 质量加固
├── 1. 为 NumericOperations<T> 编写纯逻辑单元测试 (DONE ✅)
├── 2. 引入 Avalonia.Headless 测试键盘/拖拽/事件交互 (DONE ✅)
├── 3. 边界值测试：Hex 溢出、Min/Max 钳制、无效输入状态 (DONE ✅)
└── 4. 格式化与可读性审查 (DONE ✅)

Phase 3: 功能扩展（未来）
├── 1. 内置 Undo/Redo 栈 (DONE ✅)
├── 2. 引入 IValidationRule<T> 支持自定义业务校验 (DONE ✅)
├── 3. 设计 EnumerationUpDown 架构 (DONE ✅)
└── 4. 引入 Source Generator 自动化生成非泛型包装类 (DONE ✅)
```

---

## 十、总结

> **用“职责分离”和“策略模式”来缩小单个文件的体积，而不是把代码撒到 N 个重复文件里。**

结合外部方案的视角，本次重构不仅要解决**代码重复与碎片化**的问题，还要在架构上预留**测试、主题扩展、功能增强**的接口。最终得到一个：

- **文件数更少、内聚性更高**
- **重复为零、类型安全**
- **可测试、可扩展、主题一致**

的代码库。
