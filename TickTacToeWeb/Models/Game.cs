using System.Timers;

namespace TickTacToeWeb.Models;

public class Game
{
    public const double outdatedMiliseconds = 600000;
    public enum CellType
    {
        None = 0,
        X,
        O,
    }
    public readonly Guid Id = Guid.NewGuid();

    public CellType[] board = new CellType[3 * 3];

    public CellType current
    {
        get;
        private set;
    } = CellType.X;

    public readonly System.Timers.Timer timer = new System.Timers.Timer(outdatedMiliseconds);

    public Game(Func<Game, Task> e)
    {
        timer.Elapsed += (a, b) =>
        {
            timer.Close();
            e(this);
        };
        timer.Start();
    }

    public CellType PlayNext(int x, int y)
    {

        try
        {
            board[x + y * 3] = current;
            current = current == CellType.X ? CellType.O : CellType.X;
        }
        catch(Exception)
        {
            return CellType.None;
        }

        for(int i = 0; i < 3; i++)
        {
            if(board[i * 3] == board[i * 3 + 1] && board[i * 3 + 1] == board[i * 3 + 2])
            {
                return board[i * 3];
            }
        }
        for(int i = 0; i < 3; i++)
        {
            if(board[i] == board[i + 3] && board[i + 3] == board[i + 6])
            {
                return board[i];
            }
        }
        if(board[0] == board[4] && board[4] == board[8])
        {
            return board[0];
        }
        else if(board[2] == board[4] && board[4] == board[6])
        {
            return board[2];
        }


        return CellType.None;
    }






}
