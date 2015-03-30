using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

namespace ConnectFour
{

    public class AI
    {
        public static int BestMove(List<List<SquareSimple>> board, Turn t, int count, int winCount)
        {
            Turn turn = t;

            StateSpace rootSpace = new StateSpace
            {
                Board = board,
                BoardList = new List<StateSpace>(),
                BoardFitness = 0
            };
            rootSpace = EnumerateSpace(rootSpace, turn, winCount, count);

            int bestMove = GetMiniMax(rootSpace, t, winCount);

            return bestMove;
        }

        private static int GetMiniMax(StateSpace rootSpace, Turn t, int winCount)
        {
            int index = 0;
            
            GetMax(rootSpace, t, winCount);
            double? max = rootSpace.BoardList[0].BoardFitness;

            for(int i = 1; i < rootSpace.BoardList.Count; i++)
            {
                if (max == null || max < rootSpace.BoardList[i].BoardFitness)
                {
                    max = rootSpace.BoardList[i].BoardFitness;
                    index = i;
                }
            }

            return index;
        }

        private static double? GetMax(StateSpace rootSpace, Turn playerColor, int winCount)
        {
            double? max = null;
            ;
            if (rootSpace.BoardList.Count > 0)
            {
                for(int i = 0 ; i < rootSpace.BoardList.Count; i ++)
                {
                    if (rootSpace.BoardList[i].BoardFitness != null)
                    {
                        rootSpace.BoardList[i].BoardFitness = GetMin(rootSpace.BoardList[i], playerColor, winCount);

                        if (rootSpace.BoardList[i].BoardFitness > 990000)
                            return rootSpace.BoardList[i].BoardFitness;

                        if (max == null || max < rootSpace.BoardList[i].BoardFitness)
                            max = rootSpace.BoardList[i].BoardFitness;
                    }
                }

                return max;
            }
            else
            {
                return Fitness(rootSpace.Board, playerColor, winCount);
            }


        }

        private static double? GetMin(StateSpace rootSpace, Turn playerColor, int winCount)
        {
            double? min = null;
            if (rootSpace.BoardList.Count > 0)
            {
                for(int i = 0; i < rootSpace.BoardList.Count; i ++)
                {
                    if (rootSpace.BoardList[i].BoardFitness != null)
                    {
                        rootSpace.BoardList[i].BoardFitness = GetMax(rootSpace.BoardList[i], playerColor, winCount);

                        if (rootSpace.BoardList[i].BoardFitness < -990000)
                            return rootSpace.BoardList[i].BoardFitness;

                        if (min == null || min > rootSpace.BoardList[i].BoardFitness)
                            min = rootSpace.BoardList[i].BoardFitness;
                    }
                }

                return min;
            }
            else
            {
                return Fitness(rootSpace.Board, playerColor, winCount);
            }
        }

        private static StateSpace EnumerateSpace(StateSpace space, Turn t, int winCount, int count)
        {

            for (int i = 0; i < space.Board[0].Count; i++)
            {
                Stopwatch cloneStopwatch = new Stopwatch();
                List<List<SquareSimple>> boardWithPlay = Clone(space.Board);
                StateSpace toAdd = new StateSpace();

                if (!MakePlay(i, ref boardWithPlay, t))
                    toAdd.BoardFitness = null;
                else
                    toAdd.BoardFitness = 0;
                
                toAdd.Board = boardWithPlay;
                
                toAdd.BoardList = new List<StateSpace>();
                space.BoardList.Add(toAdd);
                
            }

            Turn nextTurn = (t == Turn.Black) ? Turn.Red : Turn.Black;

            if(count > 0)
                for (int i = 0; i < space.BoardList.Count; i++)
                    space.BoardList[i] = EnumerateSpace(space.BoardList[i], nextTurn, winCount, count - 1);

            return space;
        }


        private static List<List<SquareSimple>> Clone(List<List<SquareSimple>> board)
        {
            List<List<SquareSimple>> newBoard = new List<List<SquareSimple>>();

            for (int i = 0; i < board.Count; i ++)
            {
                newBoard.Add(new List<SquareSimple>());
                for (int j = 0; j < board[i].Count; j++)
                {
                    SquareSimple s = board[i][j].Clone();
                    newBoard[i].Add(s);
                }
            }

            return newBoard;
        }

        private static double Fitness(List<List<SquareSimple>> board, Turn playerColor, int winCount)
        {
            //Printer.PrintBoard(board);

            double boardValue = 0;
            SquareState playerState;

            if(playerColor == Turn.Red)
                playerState = SquareState.Red;
            else
                playerState = SquareState.Black;

            //favor center of the board positions
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board[i].Count; j++)
                {
                    double value = (double)j/(double)board[i].Count;

                    if (board[i][j].State == playerState)
                        boardValue += (-(value * value) + value) * 20;
                    else if(board[i][j].State != SquareState.Blank)
                        boardValue -= (-(value * value) + value) * 20;


                    boardValue += CheckSpot(board, i, j, winCount, playerState);
                    

                }
            }

            
            return boardValue;
        }

        private static int CheckSpot(List<List<SquareSimple>> board, int row, int col, int winCount, SquareState playerState)
        {
            SquareState startState = board[row][col].State;

            int fitness = 0;

            bool win = false;

            if (startState == SquareState.Blank)
                return 0;

            //check Vertically
            if (row <= board.Count - winCount)
            {
                int rowCount = 1;
                bool blocked = false;
                for (int i = 1; i < winCount; i++)
                {
                    if (board[row + i][col].State == SquareState.Blank) 
                        continue;
                    else if (board[row + i][col].State != startState)
                    {
                        if (!HasRoom(board, row, col, Direction.Vertical, winCount - i))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    else
                    {
                        rowCount++;
                    }
                    
                }

                if (!blocked)
                {
                    if (rowCount == winCount)
                    {
                        win = true;
                        fitness += 1000000;
                    }

                    fitness += rowCount * rowCount * 25;
                }
            }

            //check horizontally
            if (col <= board[0].Count - winCount && !win)
            {
                int rowCount = 1;
                bool blocked = false;

                for (int i = 1; i < winCount; i++)
                {
                    if (board[row][col + i].State == SquareState.Blank)
                        continue;
                    else if (board[row][col + i].State != startState)
                    {
                        if (!HasRoom(board, row, col, Direction.Horizontal, winCount - i))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    else
                    {
                        rowCount++;
                    }
                }

                if (!blocked)
                {
                    if (rowCount == winCount)
                    {
                        win = true;
                        fitness += 1000000;
                    }
                    fitness += rowCount * rowCount * 25;
                }
            }

            //Check Diagonally going down
            if (row <= board.Count - winCount && col <= board[0].Count - winCount && !win) 
            {
                int rowCount = 1;
                bool blocked = false;

                for (int i = 1; i < winCount; i++)
                {
                    if (board[row + i][col + i].State != startState)
                        continue;
                    else if (board[row + i][col + i].State != startState)
                    {
                        if (!HasRoom(board, row, col, Direction.DiagonalDown, winCount - i))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    else
                    {
                        rowCount++;
                    }
                }

                if (!blocked)
                {
                    if (rowCount == winCount)
                    {
                        win = true;
                        fitness += 1000000;
                    }
                    fitness += rowCount * rowCount * 25;
                }
            }

            //Check Diagonally going up
            if (row - winCount >= 0 && col <= board[0].Count - winCount && !win)
            {
                int rowCount = 1;
                bool blocked = false;

                for (int i = 1; i < winCount; i++)
                {
                    if (board[row - i][col + i].State != startState)
                        continue;
                    else if (board[row - i][col + i].State != startState)
                    {
                        if (!HasRoom(board, row, col, Direction.DiagonalUp, winCount - i))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    else
                    {
                        rowCount++;
                    }
                }

                if (!blocked)
                {
                    if (rowCount == winCount)
                    {
                        win = true;
                        fitness += 1000000;
                    }
                    fitness += rowCount * rowCount * 25;
                }
            }



            if (playerState == startState)
                return fitness;
            else
                return -fitness;
        }

        private static bool HasRoom(List<List<SquareSimple>> board, int row, int col, Direction direction, int numSpaces)
        {
            bool hasRoom = true;
            switch (direction)
            {
                case Direction.Vertical:
                    if (row - numSpaces < 0)
                        return false;

                    for (int i = 1; i <= numSpaces; i++)
                        if (board[row - i][col].State != SquareState.Blank)
                            return false;
                    break;

                case Direction.Horizontal:
                    if (col - numSpaces < 0)
                        return false;

                    for(int i = 1; i <= numSpaces; i++)
                        if (board[row][col - i].State != SquareState.Blank)
                            return false;
                    break;
                case Direction.DiagonalDown:
                    if (col - numSpaces < 0 || row + numSpaces > board.Count - 1)
                        return false;

                    for(int i = 1; i <= numSpaces; i++)
                        if (board[row - i][col - i].State != SquareState.Blank)
                            return false;
                    break;
                case Direction.DiagonalUp:
                    if (col - numSpaces < 0 || row - numSpaces < 0)
                        return false;

                    for(int i = 1; i <= numSpaces; i++)
                        if (board[row + i][col - i].State != SquareState.Blank)
                            return false;
                    break;
            }

            return hasRoom;
        }

        private static bool MakePlay(int col, ref List<List<SquareSimple>> board, Turn turn)
        {
            SquareSimple square = null;

            int row = 0;

            for (int i = 0; i < board.Count; i++)
            {
                if (board[i][col].State == SquareState.Blank)
                    square = board[i][col];

            }

            //If the row is full or the square given was invalid, do nothing
            if (square == null || square.State != SquareState.Blank)
                return false;

            if (turn == Turn.Red)
                square.State = SquareState.Red;
            else
                square.State = SquareState.Black;

            return true;

        }

       
    }

        public class StateSpace
        {
            public List<List<SquareSimple>> Board { get; set; }
            public double? BoardFitness { get; set; }
            public List<StateSpace> BoardList { get; set; }
        }
}
