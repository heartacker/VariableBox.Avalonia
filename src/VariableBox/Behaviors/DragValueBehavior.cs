using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace VariableBox.Behaviors;

public class DragValueBehavior
{
    private readonly VariableBox.NumericUpDown _parent;
    private readonly Panel _dragPanel;
    private readonly TextBox _textBox;
    private Point? _point;

    public DragValueBehavior(VariableBox.NumericUpDown parent, Panel dragPanel, TextBox textBox)
    {
        _parent = parent;
        _dragPanel = dragPanel;
        _textBox = textBox;
    }

    public void Attach()
    {
        if (_dragPanel == null) return;
        _dragPanel.PointerPressed += OnPointerPressed;
        _dragPanel.PointerMoved += OnPointerMoved;
        _dragPanel.PointerReleased += OnPointerReleased;
    }

    public void Detach()
    {
        if (_dragPanel == null) return;
        _dragPanel.PointerPressed -= OnPointerPressed;
        _dragPanel.PointerMoved -= OnPointerMoved;
        _dragPanel.PointerReleased -= OnPointerReleased;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _point = e.GetPosition(_parent);
        if (e.ClickCount == 2 && _dragPanel is not null && _parent.IsAllowDrag)
        {
            _dragPanel.IsVisible = false;
            _textBox?.Focus();
            if (_textBox != null) _textBox.IsReadOnly = _parent.IsReadOnly;
        }
        else
        {
            _textBox?.Focus();
            if (_textBox != null) _textBox.IsReadOnly = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _point = null;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_parent.IsAllowDrag || _parent.IsReadOnly)
            return;
        if (!e.GetCurrentPoint(_parent).Properties.IsLeftButtonPressed)
            return;

        var point = e.GetPosition(_parent);
        if (_point == null) return;
        
        var delta = point - _point.Value;
        int d = GetDelta(delta);
        if (d > 0)
        {
            if (_parent._canIncrease) _parent.Increase();
        }
        else if (d < 0)
        {
            if (_parent._canDecrease) _parent.Decrease();
        }
        _point = point;
    }

    private int GetDelta(Point point)
    {
        bool horizontal = Math.Abs(point.X) > Math.Abs(point.Y);
        var value = horizontal ? point.X : -point.Y;
        return value switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
    }
}