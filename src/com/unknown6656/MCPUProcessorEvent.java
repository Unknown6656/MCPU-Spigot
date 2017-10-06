package com.unknown6656;

public interface MCPUProcessorEvent<T>
{
    public void Raise(MCPUProcessor proc, T data);
}
