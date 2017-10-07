package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Halt extends MCPUOpcode
{
    @Override
    public int MinimumStackSize()
    {
        return 0;
    }
    
    @Override
    public boolean Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        return false;
    }
}
