package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Mov extends MCPUOpcode
{
    @Override
    public int MinimumStackSize()
    {
        return 2;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int dst = frame.Pop();
        int src = frame.Pop();
        int val = proc.Memory(src);

        proc.Memory(dst, val);
    }
}
