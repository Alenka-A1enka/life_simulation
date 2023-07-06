using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ИИ_ЛР8
{
    public class Agent
    {
        public int x { get; set; }
        public int y { get; set; }
        public int type { get; set; }
        public static Field field;
        private double[,] weights; //размер весов 12 на 4
        public int Energy { get; private set; } //кол-во энергии
        public bool IsAlive { get; private set; } //жив ли агент
        public Agent child { get; private set; } //ребенок

        //0 - смотрит наверх
        //1 - смотрит направо
        //2 - смотрит вниз
        //3 - смотрит налево
        public int orient { get; private set; } //ориентация в пространстве


        public Agent(int x, int y, int type)
        {
            IsAlive = true;
            Energy = 5;
            child = null;

            double[,] new_weights = new double[12, 4];
            Random r = new Random();
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    new_weights[i, j] = r.Next(-99, 100) / 100.0;
                }
            }
            weights = new_weights;
            orient = 0;

            this.x = x;
            this.y = y;
            this.type = type;
        }
        public Agent(double[,] weights, int x, int y, int type)
        {
            this.weights = weights;
            IsAlive = true;
            Energy = 5;
            child = null;
            orient = 0;

            this.x = x;
            this.y = y;
            this.type = type;
        }


        public void Decision(int[] front, int[] left, int[] right, int[] near, ref string result)
        {

            //при следующем действии, ссылку на ребенка уже получить невозможно
            child = null;

            //если существо метрво или такого входа нет
            if (IsAlive == false)
            {
                //Console.WriteLine("Существо мертво");
                result += "\nСущество мертво";

                return;
            }

            //массив, который хранит все возможные принятые решения на основе восприятия мира
            double[] actives = new double[4];
            for (int i = 0; i < actives.Length; i++)
            {
                actives[i] = 0;
            }

            //создание 12 входов
            //1 - растение
            //2 - травоядное
            //3 - хищник
            //0 - пусто

            int[] input = new int[12];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = 0;
            }

            for (int i = 0; i < front.Length; i++)
            {
                if (front[i] == 2) input[0] = 1;
                if (front[i] == 3) input[1] = 1;
                if (front[i] == 1) input[2] = 1;
            }
            for (int i = 0; i < right.Length; i++)
            {
                if (right[i] == 2) input[6] = 1;
                if (right[i] == 3) input[7] = 1;
                if (right[i] == 1) input[8] = 1;
            }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] == 2) input[3] = 1;
                if (left[i] == 3) input[4] = 1;
                if (left[i] == 1) input[5] = 1;
            }
            for (int i = 0; i < near.Length; i++)
            {
                if (near[i] == 2) input[9] = 1;
                if (near[i] == 3) input[10] = 1;
                if (near[i] == 1) input[11] = 1;
            }

            actives = neural_network(input, weights);



            //поиск наиболее популярного решения
            double max = -100;
            for (int i = 0; i < actives.Length; i++)
            {
                if (actives[i] > max) max = actives[i];
                //result += actives[i] + "; ";
            }

            int decision = Array.IndexOf(actives, max);

            //Form1.PrintMassive(actives);

            //Console.WriteLine("Принятое решение: " + decision);
            //result += "\nПринятое решение: " + decision;
            Active(decision, ref result);
        }
        private void Active(int out_active, ref string result)
        {
            PrintCurrentActive(out_active, ref result);

            //есть ли возможность что-то съесть
            if (out_active == 3)
            {

                bool eat_is_possible = field.EatIsPossible(x, y, ref result);

                if (out_active == 3 && eat_is_possible) Eat(ref result);
                else EnergyLoss(ref result);
            }
            else EnergyLoss(ref result);

            //если действие переместиться
            if (out_active == 2)
            {
                field.Move(x, y);
            }
            //если действие повернуть направо
            if (out_active == 1)
            {
                orient++;
                if (orient == 4) orient = 0;
            }

            //если действие повернуть налево
            if (out_active == 0)
            {
                orient--;
                if (orient == -1) orient = 3;
            }
        }
        private int Current_active(int in_active)
        {
            //одно из возможных принятых решений
            double[] out_actives = new double[4];
            //подсчет значения всех выходов
            for (int j = 0; j < out_actives.Length; j++)
            {
                out_actives[j] = 1 * weights[in_active, j];
            }
            //поиск максимального числа из массива
            double max = out_actives.Max();

            //выполняемое действие
            int out_active = Array.IndexOf(out_actives, max);

            return out_active;
        }
        private double[] neural_network(int[] input, double[,] weights)
        {
            double[] output = new double[4];
            for (int i = 0; i < output.Length; i++)
            {
                double[] matrix = new double[12];
                for (int j = 0; j < matrix.Length; j++)
                {
                    matrix[j] = weights[j, i];
                }
                output[i] = w_sum(input, matrix);
            }
            return output;
        }
        private double w_sum(int[] a, double[] b)
        {
            double output = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                output += Convert.ToDouble(a[i]) * b[i];
            }
            return output;
        }
        private void EnergyLoss(ref string result)
        {
            Energy -= 1;

            //если энергия упала до 0 агент умирает
            if (Energy == 0)
            {
                IsAlive = false;
            }
            result += "Была потеряна энергия. \nТекущее кол-во: " + Energy;
        }
        private void Eat(ref string result)
        {
            //энергия увеличивается
            Energy += 1;
            result += "\nЭнергия прибавилась. Кол-во: " + Energy;

            //если энергия достигла 100 процентов
            if (Energy == 10)
            {
                //веса потомка
                double[,] new_weights = new double[12, 4];
                Random r = new Random();

                //новые веса с изменением от 1 до 15 процентов
                for (int i = 0; i < weights.GetLength(0); i++)
                {
                    for (int j = 0; j < weights.GetLength(1); j++)
                    {
                        double delta = r.Next(-15, 15) / 100.0;
                        new_weights[i, j] = weights[i, j] - weights[i, j] * delta;
                    }
                }

                //деление энергии поплам
                Energy = 5;

                int[] new_x_y = field.GetFreePlace(x, y);
                child = new Agent(new_weights, new_x_y[0], new_x_y[1], type);
                result += "\nАгент размножился";
                result += "\nТекущая энергия агента: 5";
            }
        }
        public void PrintWeights(string agent_name)
        {
            Console.WriteLine(agent_name);
            for (int i = 0; i < weights.GetLength(0); i++)
            {
                for (int j = 0; j < weights.GetLength(1); j++)
                {
                    Console.Write(weights[i, j] + "   ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        private void PrintCurrentActive(int active, ref string result)
        {
            result += "\nТекущее действие: ";

            if (active == 0) result += "Повернуть налево\n";
            if (active == 1) result += "Повернуть направо\n";
            if (active == 2) result += "Переместиться\n";
            if (active == 3) result += "Съесть\n";
        }
    }
}
