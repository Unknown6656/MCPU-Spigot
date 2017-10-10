package com.unknown6656.opcodes;

import java.util.function.BiFunction;


class BinaryBooleanOpcode extends BinaryOpcode
{
    public BinaryBooleanOpcode(BiFunction<Integer, Integer, Boolean> func)
    {
        super((x, y) -> func.apply(x, y) ? 0xffffffff : 0);
    }
}
