package com.unknown6656;

public abstract class MCPUOpcode
{
    public int MinimumArgumentCount()
    {
        return 0;
    }
    
    public int MinimumStackSize()
    {
        return -1;
    }
    
    public abstract boolean Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc);
}
