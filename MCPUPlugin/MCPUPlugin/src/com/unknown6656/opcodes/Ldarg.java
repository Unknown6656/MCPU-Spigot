package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Ldarg extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int arg = frame.Arguments[arguments[0]];
        
        frame.Push(arg);
    }
}