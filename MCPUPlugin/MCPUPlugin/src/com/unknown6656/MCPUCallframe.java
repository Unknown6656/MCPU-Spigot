package com.unknown6656;

import java.util.Stack;


public final class MCPUCallframe
{
    private final Stack<Integer> Stack = new Stack<>();
    public int[] Arguments = new int[0];
    public int[] Locals = new int[0];
    

    public MCPUCallframe()
    {
        this(new int[0], new int[0], new int[0]);
    }

    public MCPUCallframe(int[] arguments, int[] locals, int[] stack)
    {
        if (arguments != null)
            Arguments = arguments;

        if (locals != null)
            Locals = locals;

        if (stack != null)
            for (int i = 0; i < stack.length; ++i)
                Stack.push(stack[i]);
    }
    
    public int Pop()
    {
        return Stack.pop();
    }

    public int Peek()
    {
        return Stack.peek();
    }

    public void Push(int val)
    {
        Stack.push(val);
    }

    
    public int StackSize()
    {
        return Stack.size();
    }
}
