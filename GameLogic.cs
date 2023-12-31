﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Koursach_Tri_v_Ryad
{
    public class GameLogic
    {
        Element[,] gamefield = new Element[w, w]; 

        int X = -1;
        int Y = -1;

        const int w = 8; 
        const int nulltipe = -99; 
 
        int gamefield1 = -1;
        int gamefield2 = -1;

        bool gamefieldzamena; 
        public int score { get; set; } 

        const int missscore = 5 * ((w - 2) * 3 * w * 2);

        public void GameSetScore(int score)
        {
            this.score = score;
        }

        List<Element> SovpadEl = new List<Element>();

        Random rng = new Random();

        public EventHandler Falled;

        private void FallCellsss()
        {
            TriVRyad();
            if (Hasnullpic())
            {
                while (Hasnullpic())
                {
                    FallCells();
                    Falled(this, null);
                    Thread.Sleep(400);
                }
                StartFall();
            }
        }

        public void StartFall()
        {
            Thread newThread = new Thread(new ThreadStart(FallCellsss));
            newThread.Start();
        }

        public GameLogic(Element[,] gamefield)
        {
            this.gamefield = gamefield;
        }

        public int getScore()
        {
            return score;
        }




        public void moveCell(int i, int j)
        {
            SovpadEl.Clear(); 
            
            if ((X == -1) && (Y == -1))
            {
                X = i;
                Y = j;
                gamefield1 = gamefield[i, j].typeofpic;
            }
            else
            {
                if (((X == i) && (Math.Abs(Y - j) == 1)) || ((Y == j) && (Math.Abs(X - i) == 1)))
                {
                    gamefield2 = gamefield[i, j].typeofpic;

                    gamefield[X, Y].typeofpic = gamefield2;
                    gamefield[i, j].typeofpic = gamefield1;
               
                    List<Element> row = TriVRyad();

                    if (row.Count() != 0)
                    {
                        TriVRyad();
                    }

                    gamefieldzamena = true;
                }
                else
                {
                    gamefieldzamena = false;
                }
                if (gamefieldzamena == true)
                {
                    X = -1;
                    Y = -1;

                    gamefield1 = -1;
                    gamefield2 = -1;

                    StartFall();
                }
            }
        }
        public List<Element> TriVRyad()
        {
            SovpadEl.Clear(); 

            int count; 

            for (int i = 0; i < w; i++)
            {
                int type = gamefield[0, i].typeofpic;
                count = 1;
                for (int j = 0; j < w; j++)
                {
                   
                    if ((type == gamefield[i, j].typeofpic) && (j != 0))
                        count++;
                    else
                        count = 1;
                    
                    if (count > 2)
                    {
                        SovpadEl.Add(gamefield[i, j - 2]);
                        SovpadEl.Add(gamefield[i, j - 1]);
                        SovpadEl.Add(gamefield[i, j]);
                    }
                    type = gamefield[i, j].typeofpic; 
                }
            }

            for (int j = 0; j < w; j++)
            {
                int type = gamefield[j, 0].typeofpic;
                count = 1;
                for (int i = 0; i < w; i++)
                {
                    if ((type == gamefield[i, j].typeofpic) && (i != 0))
                        count++;
                    else
                        count = 1;
                    if (count > 2)
                    {
                        SovpadEl.Add(gamefield[i - 2, j]);
                        SovpadEl.Add(gamefield[i - 1, j]);
                        SovpadEl.Add(gamefield[i, j]);
                    }
                    type = gamefield[i, j].typeofpic;
                }
            }

            foreach (Element elem in SovpadEl)
            {
                elem.typeofpic = nulltipe;
            }

            score += SovpadEl.Count() * 5;

            return SovpadEl;
        }

        public bool Hasnullpic()
        {
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    if (gamefield[i, j].typeofpic == nulltipe) return true;
                }
            }
            return false;
        }

        public void FallCells()
        {
            int typeel = -1;

            for (int j = w - 1; j >= 0; j--)
            {
                for (int i = w - 2; i >= 0; i--)
                {
                    if (gamefield[i + 1, j].typeofpic == nulltipe)
                    {
                        typeel = gamefield[i, j].typeofpic;

                        gamefield[i + 1, j].typeofpic = typeel;
                        gamefield[i, j].typeofpic = nulltipe;
                    }
                }
            }
            for (int j = 0; j < w; j++)
                for (int i = 0; i < w; i++)
                {
                    if (gamefield[i, j].typeofpic == nulltipe)
                    {
                        gamefield[i, j].typeofpic = rng.Next(0, 6);
                    }
                    break;
                }
        }
    }
}
