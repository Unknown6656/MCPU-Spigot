package epsilonpotato.mcpu.mcpuarch.opcodes;

import java.util.Random;

import org.bukkit.Location;
import org.bukkit.Material;
import org.bukkit.Particle;
import org.bukkit.World;
import org.bukkit.block.Block;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;
import epsilonpotato.mcpu.mcpuarch.MCPUPlugin;


public final class Abk extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 0;
    }
    
    @Override
    public int MinimumStackSize()
    {
        return 0;
    }
    
    @Override
    @SuppressWarnings("deprecation") // TODO : fix this
    public void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int num = -1;
        
        synchronized (MCPUPlugin.circuits)
        {
            for (int i : MCPUPlugin.circuits.keySet())
                if (MCPUPlugin.circuits.get(i) == proc)
                {
                    num = i;
                    
                    break;
                }
            
            MCPUPlugin.circuits.remove(num);
        }

        Location pos = proc.getCenterLocation();
        int soffs = proc.getIOCount();
        World w = proc.getWorld();
        
        proc.onError.Raise(proc, " BAAAM !!! ");
        proc.reset(true);
        
        w.createExplosion(pos.getBlockX(), pos.getBlockY(), pos.getBlockZ(), 4f, true, true);
        w.spawnParticle(Particle.SMOKE_LARGE, pos.getBlockX(), pos.getBlockY() + 2, pos.getBlockZ(), 60000 /* six gorillion * 1e-2 particles !! */, 2, 10, 2, 0);

        Random r = new Random();
        final double md = Math.sqrt(soffs * soffs * 2.25 + 5);
        
        for (int i = 1 - soffs; i <= soffs; ++i)
            for (int j = 1 - soffs; j <= soffs; ++j)
                for (int h = 1; h >= -1; --h)
                {
                    double d = Math.sqrt(i * i + j * j + h * h);
                    Block b = w.getBlockAt(pos.getBlockX() + i, pos.getBlockY() + h, pos.getBlockZ() + j);
                    
                    if (r.nextFloat() > d / md)
                        b.breakNaturally();
                    else if (b.getType() == Material.WOOL)
                        if (r.nextBoolean())
                        {
                            Block bc = b.getRelative(0, -1, 0);
                            
                            bc.setType(Material.WOOL);
                            bc.setData(b.getData());
                            
                            b.setType(Material.AIR);
                        }
                }
    }
}
