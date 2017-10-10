package com.unknown6656;

import java.util.function.BiFunction;


public class BinaryBooleanOpcode extends BinaryOpcode
{
    public BinaryBooleanOpcode(BiFunction<Integer, Integer, Boolean> func)
    {
        super((x, y) -> func.apply(x, y) ? 0xffffffff : 0);
    }
}
