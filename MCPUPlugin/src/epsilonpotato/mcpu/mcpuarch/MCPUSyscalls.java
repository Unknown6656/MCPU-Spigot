package epsilonpotato.mcpu.mcpuarch;

import java.util.HashMap;

import org.bukkit.ChatColor;


public class MCPUSyscalls
{
    public static final HashMap<Integer, Syscall> syscalls = new HashMap<>();
    
    static
    {
        syscalls.put(1, (f, p, a) -> MCPUPlugin.print(p.getCreator(), ChatColor.WHITE, String.format("%d (0x%08x)", f.Peek(), f.Peek())));
        syscalls.put(2, (f, p, a) ->
        {
            StringBuilder sb = new StringBuilder();
            final int width = 8;
            
            for (int i = 0, l = p.getMemorySize(); i < l; ++i)
            {
                String data = String.format("%08x", p.getMemory(i));

                sb.append(data + (((i % width) == 0) && (i > 0) ? '\n' : ' '));
            }

            MCPUPlugin.print(p.getCreator(), ChatColor.WHITE, sb.toString());
        });
        syscalls.put(0, (f, p, a) ->
        {
            byte[] bytes = new byte[a.length * 4];
            
            for (int i = 0; i < a.length; ++i)
            {
                bytes[i * 4] = (byte)((a[i] >>> 24) & 0xff);
                bytes[i * 4 + 1] = (byte)((a[i] >> 16) & 0xff);
                bytes[i * 4 + 2] = (byte)((a[i] >> 8) & 0xff);
                bytes[i * 4 + 3] = (byte)(a[i] & 0xff);
            }

            MCPUPlugin.print(p.getCreator(), ChatColor.WHITE, new String(bytes)); 
        });
    }
    
    
    public interface Syscall
    {
        public void Execute(MCPUCallframe frame, MCPUProcessor proc, int[] args);
    }
}