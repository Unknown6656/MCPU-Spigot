package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Stiodir extends MCPUOpcode
{
    // stiodir port dir
    @Override
    public int MinimumStackSize()
    {
        return 2;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int direction = frame.Pop();
        int port = frame.Pop();
        
        proc.SetIODirection(port, direction != 0);
    }
}
