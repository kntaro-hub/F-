
/// <summary>
/// 部屋同士の接続状態
/// </summary>
enum ConnectInfo
{
    prev,
    id,
    next,
    max
}

/// <summary>
/// ダンジョン1区画情報
/// </summary>
public class Division
{
    private int[] connectNum = new int[3];
    public int[] ConnectNum
    {
        get { return connectNum; }
        set { connectNum = value; }
    }

    /// <summary>
    /// 矩形管理
    /// </summary>
    public class Div_Room
    {
        public int Left   = 0; // 左
        public int Top    = 0; // 上
        public int Right  = 0; // 右
        public int Bottom = 0; // 下

        public int RoomNumber = 0;  // 部屋番号

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Div_Room()
        {
            
        }
        /// <summary>
        /// 値をまとめて設定する
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="top">上</param>
        /// <param name="right">右</param>
        /// <param name="bottom">左</param>
        public void Set(int left, int top, int right, int bottom)
        {
            Left   = left;
            Top    = top;
            Right  = right;
            Bottom = bottom;
        }
        /// <summary>
        /// 幅
        /// </summary>
        public int Width
        {
            get { return Right - Left; }
        }
        /// <summary>
        /// 高さ
        /// </summary>
        public int Height
        {
            get { return Bottom - Top; }
        }
        /// <summary>
        /// 面積 (幅 x 高さ)
        /// </summary>
        public int Measure
        {
            get { return Width * Height; }
        }

        /// <summary>
        /// 矩形情報をコピーする
        /// </summary>
        /// <param name="rect">コピー元の矩形情報</param>
        public void Copy(Div_Room rect)
        {
            Left   = rect.Left;
            Top    = rect.Top;
            Right  = rect.Right;
            Bottom = rect.Bottom;
        }
    }

    /// <summary>
    /// 外周の矩形（区画）情報
    /// </summary>
    public Div_Room Outer;
    /// <summary>
    /// 区画内に作ったルーム情報
    /// </summary>
    public Div_Room Room;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Division()
    {
        Outer = new Div_Room();
        Room = new Div_Room();
    }
}
