using UnityEngine;


public class LevelStructure
{
    
    public int width;
    public int depth;
    public int playfieldDepth;
    public int partIndex;
    public CubeModel.CubeType[,] cells;

    public string GetCellsString()
    {
        string ret = null;
        
        for(int i=0; i< cells.GetLength(0); i++)
        {
            for(int j=0; j<cells.GetLength(1); j++)
            {
                ret += cells[i,j].ToString("f") + " / ";
            }
            ret += "\n";
        }
        return ret;
    }

}
