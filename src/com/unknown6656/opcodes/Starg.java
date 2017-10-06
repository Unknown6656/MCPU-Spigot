package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Starg extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public boolean Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int arg = arguments[0];
        int val = frame.Pop();
        
        frame.Arguments[arg] = val;
        
        return true;
    }
}
