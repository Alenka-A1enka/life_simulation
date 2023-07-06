using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ИИ_ЛР8
{
    public class Field
    {
        //1 - растение
        //2 - травоядное
        //3 - хищник
        //0 - пусто

        public Agent[,] field;
        public int size;
        public Field(int size, int plant_count, int herbivore_count, int predator_count)
        {
            field = new Agent[size, size];
            this.size = size;
            Random r = new Random();

            //добавление везде нулевых агентов
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    field[i, j] = new Agent(i, j, 0);
                }
            }


            //добавление сущностей на поле
            int count = 0;
            for (int i = 0; i < plant_count; i++)
            {
                int x = r.Next(0, size);
                int y = r.Next(0, size);

                while (true)
                {
                    try
                    {
                        if (field[x, y].type == 0)
                        {
                            field[x, y] = new Agent(x, y, 1);
                            count++;
                            break;
                        }
                        else
                        {
                            int z = r.Next(0, 2);
                            if (z == 0) x += 1;
                            else y += 1;
                        }
                    }
                    catch { x = 0; y = 0; }
                }
            }


            count = 0;
            for (int i = 0; i < herbivore_count; i++)
            {
                int x = r.Next(0, size);
                int y = r.Next(0, size);

                while (true)
                {
                    try
                    {
                        if (field[x, y].type == 0)
                        {
                            field[x, y] = new Agent(x, y, 2);
                            count++;
                            break;
                        }
                        else
                        {
                            int z = r.Next(0, 2);
                            if (z == 0) x += 1;
                            else y += 1;
                        }
                    }
                    catch { x = 0; y = 0; }
                }
            }


            count = 0;
            for (int i = 0; i < predator_count; i++)
            {
                int x = r.Next(0, size);
                int y = r.Next(0, size);

                while (true)
                {
                    try
                    {
                        if (field[x, y].type == 0)
                        {
                            field[x, y] = new Agent(x, y, 3);
                            count++;
                            break;
                        }
                        else
                        {
                            int z = r.Next(0, 2);
                            if (z == 0) x += 1;
                            else y += 1;
                        }
                    }
                    catch { x = 0; y = 0; }
                }
            }

        }

        public void GetFront(int x, int y, ref int[] front, ref int[] left, ref int[] right, ref int[] near)
        {
            if (x >= size || y >= size || x < 0 || y < 0) return;

            //создание расширенного поля на два элемента с каждой стороны
            int size_2 = size + 4;
            int[,] new_field = new int[size_2, size_2];

            //копирование старого поля
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    new_field[i + 2, j + 2] = field[i, j].type;
                }
            }


            //добавление новых элементов

            //1. добавялем все углы 2 на 2 на 4 стороны поля
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    new_field[i, j] = field[size - 1 - (1 - i), size - 1 - (1 - j)].type;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                for (int j = size_2 - 2; j < size_2; j++)
                {
                    new_field[i, j] = field[size - 1 - (1 - i), (2 - (size_2 - j))].type;
                }
            }
            for (int i = size_2 - 2; i < size_2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    new_field[i, j] = field[2 - (size_2 - i), size - 1 - (1 - j)].type;
                }
            }
            for (int i = size_2 - 2; i < size_2; i++)
            {
                for (int j = size_2 - 2; j < size_2; j++)
                {
                    new_field[i, j] = field[2 - (size_2 - i), 2 - (size_2 - j)].type;
                }
            }
            //добавляем оставшиеся промежутки

            for (int i = 0; i < 2; i++)
            {
                for (int j = 2; j < size_2 - 2; j++)
                {
                    new_field[i, j] = field[size - 2 + i, j - 2].type;
                }
            }
            for (int i = 2; i < size_2 - 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    new_field[i, j] = field[i - 2, j + 3].type;
                }
            }
            for (int i = 2; i < size_2 - 2; i++)
            {
                for (int j = size_2 - 2; j < size_2; j++)
                {
                    new_field[i, j] = field[i - 2, j - 7].type;
                }
            }
            for (int i = size_2 - 2; i < size_2; i++)
            {
                for (int j = 2; j < size_2 - 2; j++)
                {
                    new_field[i, j] = field[i - 7, j - 2].type;
                }
            }


            int orient = field[x, y].orient;

            x += 2;
            y += 2;
            //получение фронта
            front = SetFront(x, y, orient, new_field);

            //получение левого бока
            left = SetLeft(x, y, orient, new_field);

            //получение правого бока 
            right = SetRight(x, y, orient, new_field);

            //близость
            near = SetNear(x, y, orient, new_field);

        }

        private int[] SetFront(int x, int y, int orient, int[,] new_field)
        {
            int[] new_front = new int[5];

            if (orient == 0)
            {
                for (int i = 0; i < new_front.Length; i++)
                {
                    new_front[i] = new_field[x - 2, y - 2 + i];
                }
            }
            if (orient == 1)
            {
                for (int i = 0; i < new_front.Length; i++)
                {
                    new_front[i] = new_field[x - 2 + i, y + 2];
                }
            }
            if (orient == 2)
            {
                for (int i = 0; i < new_front.Length; i++)
                {
                    new_front[i] = new_field[x + 2, y - 2 + i];
                }
            }
            if (orient == 3)
            {
                for (int i = 0; i < new_front.Length; i++)
                {
                    new_front[i] = new_field[x - 2 + i, y - 2];
                }
            }

            return new_front;
        }
        private int[] SetLeft(int x, int y, int orient, int[,] new_field)
        {
            int[] new_left = new int[2];
            if (orient == 0)
            {
                new_left[0] = new_field[x, y - 2];
                new_left[1] = new_field[x - 1, y - 2];
            }
            if (orient == 1)
            {
                new_left[0] = new_field[x - 2, y];
                new_left[1] = new_field[x - 2, y + 1];
            }
            if (orient == 2)
            {
                new_left[0] = new_field[x, y + 2];
                new_left[1] = new_field[x + 1, y + 2];
            }
            if (orient == 3)
            {
                new_left[0] = new_field[x + 2, y - 1];
                new_left[1] = new_field[x + 2, y];
            }

            return new_left;
        }
        private int[] SetRight(int x, int y, int orient, int[,] new_field)
        {
            int[] new_right = new int[2];
            if (orient == 0)
            {
                new_right[0] = new_field[x, y + 2];
                new_right[1] = new_field[x - 1, y + 2];
            }
            if (orient == 1)
            {
                new_right[0] = new_field[x + 2, y];
                new_right[1] = new_field[x + 2, y + 1];
            }
            if (orient == 2)
            {
                new_right[0] = new_field[x, y - 2];
                new_right[1] = new_field[x + 1, y - 2];
            }
            if (orient == 3)
            {
                new_right[0] = new_field[x - 2, y - 1];
                new_right[1] = new_field[x - 2, y];
            }

            return new_right;
        }
        private int[] SetNear(int x, int y, int orient, int[,] new_field)
        {
            int[] new_near = new int[5];

            if (orient == 0)
            {
                new_near[0] = new_field[x, y - 1];
                new_near[4] = new_field[x, y + 1];

                for (int i = 1; i < 4; i++)
                {
                    new_near[i] = new_field[x - 1, y - 1 + (i - 1)];
                }
            }
            if (orient == 1)
            {
                new_near[0] = new_field[x - 1, y];
                new_near[4] = new_field[x + 1, y];

                new_near[1] = new_field[x - 1, y + 1];
                new_near[2] = new_field[x, y + 1];
                new_near[3] = new_field[x + 1, y + 1];
            }
            if (orient == 2)
            {
                new_near[0] = new_field[x, y - 1];
                new_near[4] = new_field[x, y + 1];

                new_near[1] = new_field[x + 1, y - 1];
                new_near[2] = new_field[x + 1, y];
                new_near[3] = new_field[x + 1, y + 1];
            }
            if (orient == 3)
            {
                new_near[0] = new_field[x - 1, y];
                new_near[4] = new_field[x + 1, y];

                new_near[1] = new_field[x - 1, y - 1];
                new_near[2] = new_field[x, y - 1];
                new_near[3] = new_field[x + 1, y - 1];
            }

            return new_near;
        }

        //public void PrintField()
        //{
        //    for (int i = 0; i < size; i++)
        //    {
        //        for (int j = 0; j < size; j++)
        //        {
        //            Console.Write(field[i, j].type + "   ");
        //        }
        //        Console.WriteLine();
        //        Console.WriteLine();
        //    }
        //}
        public void GetNextTact(ref string result)
        {
            result = "";
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //если это хищник или травоядное
                    if (field[i, j].type == 2 || field[i, j].type == 3)
                    {
                        int[] front = new int[5];
                        int[] left = new int[2];
                        int[] right = new int[2];
                        int[] near = new int[5];
                        result += "\n\nАгент " + i + "-" + j;
                        GetFront(i, j, ref front, ref left, ref right, ref near);
                        field[i, j].Decision(front, left, right, near, ref result);
                        //result += "\nОриентация: " + field[i, j].orient;

                        //если существо мертво
                        if (field[i, j].IsAlive == false)
                        {
                            field[i, j] = new Agent(i, j, 0);
                        }
                        //если агент размножился
                        if (field[i, j].child != null)
                        {
                            for (int k = 0; k < size; k++)
                            {
                                bool f = true;
                                for (int p = 0; p < size; p++)
                                {
                                    if (field[k, p].type == 0)
                                    {
                                        field[k, p] = new Agent(k, p, field[i, j].type);
                                        f = false;
                                        break;
                                    }
                                }
                                if (!f) break;
                            }
                        }

                    }
                }
            }
            Random r = new Random();
            int x = r.Next(0, size);
            int y = r.Next(0, size);

            while (true)
            {
                try
                {
                    if (field[x, y].type == 0)
                    {
                        field[x, y] = new Agent(x, y, 1);
                        break;
                    }
                    else
                    {
                        int z = r.Next(0, 2);
                        if (z == 0) x += 1;
                        else y += 1;
                    }
                }
                catch { x = 0; y = 0; }
            }
        }
        public bool EatIsPossible(int x, int y, ref string result)
        {
            int current_orient = field[x, y].orient;

            bool possib = false;


            //1 - растение
            //2 - травоядное
            //3 - хищник
            //0 - пусто

            if (current_orient == 0)
            {
                if (x == 0)
                {
                    //Console.WriteLine(field[size - 1, y].type);
                    if (field[size - 1, y].type == 1 && field[x, y].type == 2)
                    {
                        field[size - 1, y] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[size - 1, y].type == 2 && field[x, y].type == 3)
                    {
                        field[size - 1, y] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
                else
                {
                    //Console.WriteLine(field[x - 1, y].type);
                    if (field[x - 1, y].type == 1 && field[x, y].type == 2)
                    {
                        field[x - 1, y] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[x - 1, y].type == 2 && field[x, y].type == 3)
                    {
                        field[x - 1, y] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
            }
            else if (current_orient == 1)
            {
                if (y == size - 1)
                {
                    //Console.WriteLine(field[x, 0].type);
                    if (field[x, 0].type == 1 && field[x, y].type == 2)
                    {
                        field[x, 0] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[x, 0].type == 2 && field[x, y].type == 3)
                    {
                        field[x, 0] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
                else
                {
                    //Console.WriteLine(field[x, y + 1].type);
                    if (field[x, y + 1].type == 1 && field[x, y].type == 2)
                    {
                        field[x, y + 1] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[x, y + 1].type == 2 && field[x, y].type == 3)
                    {
                        field[x, y + 1] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
            }
            else if (current_orient == 2)
            {
                if (x == size - 1)
                {
                    //Console.WriteLine(field[0, y].type);
                    if (field[0, y].type == 1 && field[x, y].type == 2)
                    {
                        field[0, y] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[0, y].type == 2 && field[x, y].type == 3)
                    {
                        field[0, y] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
                else
                {
                    //Console.WriteLine(field[x + 1, y].type);
                    if (field[x + 1, y].type == 1 && field[x, y].type == 2)
                    {
                        field[x + 1, y] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[x + 1, y].type == 2 && field[x, y].type == 3)
                    {
                        field[x + 1, y] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
            }
            else if (current_orient == 3)
            {
                if (y == 0)
                {
                    //Console.WriteLine(field[x, size - 1].type);
                    if (field[x, size - 1].type == 1 && field[x, y].type == 2)
                    {
                        field[x, size - 1] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[x, size - 1].type == 2 && field[x, y].type == 3)
                    {
                        field[x, size - 1] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
                else
                {
                    //Console.WriteLine(field[x, y - 1].type);
                    if (field[x, y - 1].type == 1 && field[x, y].type == 2)
                    {
                        field[x, y - 1] = new Agent(0, 0, 0);
                        result += "Агент съел растение";
                        return true;
                    }
                    if (field[x, y - 1].type == 2 && field[x, y].type == 3)
                    {
                        field[x, y - 1] = new Agent(0, 0, 0);
                        result += "Агент съел травоядное животное";
                        return true;
                    }
                }
            }
            result += "Агент не питается данной пищей";
            return false;

        }
        public void Move(int x, int y)
        {
            //0 - смотрит наверх
            //1 - смотрит направо
            //2 - смотрит вниз
            //3 - смотрит налево

            int current_orient = field[x, y].orient;
            if (current_orient == 0)
            {
                //if (field[x + 1, y].type == 0)
                //{
                //    field[x + 1, y] = field[x, y];
                //    field[x, y] = new Agent(x, y, 0);
                //}
                if (x == 0)
                {
                    //Console.WriteLine(field[size - 1, y].type);
                    if (field[size - 1, y].type == 0)
                    {
                        field[size - 1, y] = field[x, y];
                        field[size - 1, y].x = size - 1;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
                else
                {
                    //Console.WriteLine(field[x - 1, y].type);
                    if (field[x - 1, y].type == 0)
                    {
                        field[x - 1, y] = field[x, y];
                        field[x - 1, y].x -= 1;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
            }
            else if (current_orient == 1)
            {
                if (y == size - 1)
                {
                    //Console.WriteLine(field[x, 0].type);
                    if (field[x, 0].type == 0)
                    {
                        field[x, 0] = field[x, y];
                        field[x, 0].y = 0;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
                else
                {
                    //Console.WriteLine(field[x, y + 1].type);
                    if (field[x, y + 1].type == 0)
                    {
                        field[x, y + 1] = field[x, y];
                        field[x, y + 1].y += 1;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
            }
            else if (current_orient == 2)
            {
                if (x == size - 1)
                {
                    //Console.WriteLine(field[0, y].type);
                    if (field[0, y].type == 0)
                    {
                        field[0, y] = field[x, y];
                        field[0, y].x = 0;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
                else
                {
                    //Console.WriteLine(field[x + 1, y].type);
                    if (field[x + 1, y].type == 0)
                    {
                        field[x + 1, y] = field[x, y];
                        field[x + 1, y].x += 1;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
            }
            else if (current_orient == 3)
            {
                if (y == 0)
                {
                    //Console.WriteLine(field[x, size - 1].type);
                    if (field[x, size - 1].type == 0)
                    {
                        field[x, size - 1] = field[x, y];
                        field[x, size - 1].y = size - 1;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
                else
                {
                    //Console.WriteLine(field[x, y - 1].type);
                    if (field[x, y - 1].type == 0)
                    {
                        field[x, y - 1] = field[x, y];
                        field[x, y - 1].y -= 1;
                        field[x, y] = new Agent(x, y, 0);
                    }
                }
            }
        }

        public int[] GetFreePlace(int x, int y)
        {
            int[] new_x_y = new int[2] { 0, 0 };
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (field[i, j].type == 0)
                    {
                        new_x_y[0] = i;
                        new_x_y[1] = j;
                        return new_x_y;
                    }
                }
            }
            return new_x_y;
        }
    }
}
