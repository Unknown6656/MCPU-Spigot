package com.unknown6656;

import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.function.Consumer;


public class Parallel
{
    static final int iCPU = Runtime.getRuntime().availableProcessors();
    
    
    public static <T> void ForEach(Iterable<T> parameters, final Consumer<T> loopBody)
    {
        ExecutorService executor = Executors.newFixedThreadPool(iCPU);
        List<Future<?>> futures = new LinkedList<Future<?>>();
        
        for (final T param : parameters)
        {
            Future<?> future = executor.submit(new Runnable()
            {
                public void run()
                {
                    loopBody.accept(param);
                }
            });
            
            futures.add(future);
        }
        
        for (Future<?> f : futures)
            try
            {
                f.get();
            }
            catch (InterruptedException e)
            {
            }
            catch (ExecutionException e)
            {
            }
        
        executor.shutdown();
    }
    
    public static void For(int start, int stop, final Consumer<Integer> loopBody)
    {
        ExecutorService executor = Executors.newFixedThreadPool(iCPU);
        List<Future<?>> futures = new LinkedList<Future<?>>();
        
        for (int i = start; i < stop; i++)
        {
            final Integer k = i;
            
            Future<?> future = executor.submit(new Runnable()
            {
                public void run()
                {
                    loopBody.accept(k);
                }
            });
            
            futures.add(future);
        }
        
        for (Future<?> f : futures)
            try
            {
                f.get();
            }
            catch (InterruptedException e)
            {
            }
            catch (ExecutionException e)
            {
            }
        
        executor.shutdown();
    }
}