﻿
int cnt;


void init(void)
{
    int i;

    while (i < cnt)
    {
        setiodir(i, 1);
        
        i += 1;
    }
}

void main(void)
{
    int i;

    i = 0;
    cnt = sizeof(io);

    init();

    while (true)
    {
        io[i] = io[i] > 0 ? 0 : 1;

        i += 1;
        i %= cnt;
    }
}
