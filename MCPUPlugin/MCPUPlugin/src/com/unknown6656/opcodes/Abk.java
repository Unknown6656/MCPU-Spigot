package com.unknown6656.opcodes;

import java.util.Random;

import org.bukkit.Material;
import org.bukkit.Particle;
import org.bukkit.World;
import org.bukkit.block.Block;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;
import com.unknown6656.Main;
import com.unknown6656.Triplet;


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
        
        synchronized (Main.cores)
        {
            for (int i : Main.cores.keySet())
                if (Main.cores.get(i) == proc)
                {
                    num = i;
                    
                    break;
                }
            
            Main.cores.remove(num);
            Main.regions.remove(num);
        }

        Triplet<Integer, Integer, Integer> pos = proc.Location;
        int soffs = proc.sideoffset;
        World w = proc.World;
        
        proc.OnError.Raise(proc, " BAAAM !!! ");
        proc.Reset(true);
        
        w.createExplosion(pos.x, pos.y, pos.y, 4f, true, true);
        w.spawnParticle(Particle.SMOKE_LARGE, pos.x, pos.y + 2, pos.z, 60000 /* six gorillion * 1e-2 particles !! */, 2, 10, 2, 0);

        Random r = new Random();
        final double md = Math.sqrt(soffs * soffs * 2.25 + 5);
        
        for (int i = 1 - soffs; i <= soffs; ++i)
            for (int j = 1 - soffs; j <= soffs; ++j)
                for (int h = 1; h >= -1; --h)
                {
                    double d = Math.sqrt(i * i + j * j + h * h);
                    Block b = w.getBlockAt(pos.x + i, pos.y + h, pos.z + j);
                    
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
