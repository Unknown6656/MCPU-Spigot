package com.unknown6656;

import java.util.HashMap;

import org.bukkit.ChatColor;


public class MCPUSyscalls
{
    public static final HashMap<Integer, Syscall> syscalls = new HashMap<>();
    
    static
    {
        syscalls.put(1, (f, p) -> Main.Print(p.Creator, ChatColor.WHITE, String.format("%d (0x%08x)", f.Peek(), f.Peek())));
        syscalls.put(2, (f, p) ->
        {
            StringBuilder sb = new StringBuilder();
            final int width = 8;
            
            for (int i = 0, l = p.MemorySize(); i < l; ++i)
            {
                String data = String.format("%08x", p.Memory(i));

                sb.append(data + (((i % width) == 0) && (i > 0) ? '\n' : ' '));
            }

            Main.Print(p.Creator, ChatColor.WHITE, sb.toString());
        });
    }
    
    
    public interface Syscall
    {
        public void Execute(MCPUCallframe frame, MCPUProcessor proc);
    }
}