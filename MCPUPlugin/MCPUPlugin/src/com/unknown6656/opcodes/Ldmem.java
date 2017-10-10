package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Ldmem extends MCPUOpcode
{
    @Override
    public final int MinimumStackSize()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int addr = frame.Pop();
        int val = proc.Memory(addr);
        
        frame.Push(val);
    }
}
