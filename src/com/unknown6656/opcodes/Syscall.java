package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;
import com.unknown6656.MCPUSyscalls;


public final class Syscall extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int num = arguments[1];
        MCPUSyscalls.Syscall call = MCPUSyscalls.syscalls.get(num);
        
        call.Execute(frame, proc);
    }
}
