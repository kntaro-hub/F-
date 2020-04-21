using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// グリッド上の座標
public struct Point
{
    public int x;   // X座標
    public int y;   // Y座標

    public Point(int x = 0, int y = 0) : this()
    {
        this.x = x;
        this.y = y;
    }

    public void SetVal(int x = 0, int y = 0)
    {// 値設定
        this.x = x;
        this.y = y;
    }
    public void SetVal(Point point)
    {// 値設定
        this.x = point.x;
        this.y = point.y;
    }

    public Point Error()
    {// エラー値
        return new Point(9999, 9999);
    }
    public static float Distance(Point point1, Point point2)
    {
        return (Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.y - point2.y, 2)));
    }

    public static bool operator !=(Point point, Point point2)
    {
        if(point.x != point2.x && point.y != point2.y)
        {
            return true;
        }
        return false;
    }
    public static bool operator ==(Point point, Point point2)
    {
        if (point.x == point2.x && point.y == point2.y)
        {
            return true;
        }
        return false;
    }

    public static bool operator ==(Point point, int p)
    {
        if (point.x == p && point.y == p)
        {
            return true;
        }
        return false;
    }
    public static bool operator !=(Point point, int point2)
    {
        if (point.x != point2 || point.y != point2)
        {
            return true;
        }
        return false;
    }
    public static Point operator +(Point point, Point point2)
    {
        return new Point(point.x + point2.x, point.y + point2.y);
    }
    public static Point operator -(Point point, Point point2)
    {
        return new Point(point.x - point2.x, point.y - point2.y);
    }
    public static Point operator *(Point point, int num)
    {
        return new Point(point.x * num, point.y * num);
    }
}

public class AStarSys : MonoBehaviour
{
    // スタート地点
    private Point startPoint = new Point();
    // ゴール
    private Point goalPoint = new Point();

    // マネージャ
    private NodeMGR nodeMGR = null;

    // 到着フラグ
    private bool IsArrival = false;

    

    public Point StartPoint
    {
        get { return startPoint; }
    }

    private void Start()
    {
        // ゴール
        goalPoint = SequenceMGR.instance.Player.status.point;

        // マネージャ作成
        nodeMGR = new NodeMGR(goalPoint.x, goalPoint.y);
    }

    public void SetStartPoint(Point point)
    {
        startPoint = point;
    }

    private AStarSys()
    {
        
    }

    // ノード一つ分
    class A_StarNode
    {
        // 状態
        private enum NodeState
        {
            none = 0,
            open,
            closed,
            max
        }
        private NodeState State = NodeState.none;

        // 実コスト
        private int cost = 0;

        // ヒューリスティックコスト
        private int heuristicCost = 0;

        // 親ノード
        private A_StarNode parentNode = null;

        // ノード座標
        private int x = 0;
        private int y = 0;

        public int X
        {// X座標を返す
            get { return this.x; }
        }
        public int Y
        {// Y座標を返す
            get { return this.y; }
        }
        public Point GetPoint()
        {// XY座標をまとめて返す
            return new Point(this.X, this.Y);
        }

        public int Cost
        {// 実コストを返す
            get { return cost; }
            set { cost = value; }
        }

        // コンストラクタ
        public A_StarNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // 総スコア計算＆返却
        public int GetGeneralCost()
        {
            if (MapData.instance.GetMapChipType(new Point(X, Y)) == MapData.MapChipType.wall)
            {
                cost = 9999;
            }

            // 実コストとヒューリスティックコストを足したものを総スコアとする
            return cost + heuristicCost;
        }

        // ヒューリスティックを計算
        public void CalcHeuristic(Point goalPoint)
        {
            // 縦横移動の場合の計算
            float wx = Mathf.Abs(goalPoint.x - this.X);
            float wy = Mathf.Abs(goalPoint.y - this.Y);

            // ヒューリスティックコスト算出
            heuristicCost = (int)(wx + wy);

            // 情報表示
            //this.ShowData();
        }

        // ステータスがnoneかどうかを返す
        public bool IsNone()
        {
            return State == NodeState.none;
        }

        public void None()
        {
            State = NodeState.none;
        }

        // ステータスをopenに変更
        public void Open(A_StarNode parent, int setCost)
        {
            //AdDebug.Log(string.Format("Open: ({0},{1})", X, Y));
            State = NodeState.open;
            cost = setCost;
            parentNode = parent;
        }

        // ステータスをclosedに変更
        public void Close()
        {
            //AdDebug.Log(string.Format("Close: ({0},{1})", X, Y));
            State = NodeState.closed;
        }

        // 親ノードをたどる
        public void GetPath(List<Point> points)
        {
            Point newPoint = new Point();
            newPoint.SetVal(X, Y);

            points.Add(newPoint);
            if(parentNode != null)
            {
                parentNode.GetPath(points);
            }
        }

        public void ShowData()
        {
            AdDebug.Log(
                string.Format("({0},{1})[{2}] cost={3} heuris={4} score={5}", 
                                X, Y, State, cost, heuristicCost, GetGeneralCost()));
        }
    }

    // ノード管理
    class NodeMGR
    {
        Actor actor;

        // オープンリスト
        List<A_StarNode> openList = null;
        // ノード管理
        Dictionary<int, A_StarNode> Nodes = null;
        // ゴール座標
        Point goal = new Point();

        public Point SetGoal
        {
            set { goal = value; }
        }

        public NodeMGR(int xgoal, int ygoal)
        {
            openList = new List<A_StarNode>();
            Nodes = new Dictionary<int, A_StarNode>();
            goal.x = xgoal;
            goal.y = ygoal;
        }
        // ノード生成する
        public A_StarNode GetNode(int x, int y)
        {
            var idx = MapData.instance.ToIdx(x, y);
            if (Nodes.ContainsKey(idx))
            {
                // 既に存在しているのでプーリングから取得
                return Nodes[idx];
            }

            // ないので新規作成
            var node = new A_StarNode(x, y);
            Nodes[idx] = node;
            // ヒューリスティックコストを計算する
            node.CalcHeuristic(goal);
            return node;
        }
        // ノードをオープンリストに追加する
        public void AddOpenList(A_StarNode node)
        {
            openList.Add(node);
        }
        // ノードをオープンリストから削除する
        public void RemoveOpenList(A_StarNode node)
        {
            openList.Remove(node);
        }
        public void ClearOpenList()
        {
            openList.Clear();
        }
        public void Reset()
        {
            foreach(var itr in openList)
            {
                itr.None();
            }
            Nodes.Clear();
        }
        // 指定の座標にあるノードをオープンする
        public A_StarNode OpenNode(int x, int y, int cost, A_StarNode parent)
        {
            if (MapData.instance.GetMapChipType(x, y) == MapData.MapChipType.wall)
            {
                // 通過できない
                return null;
            }

            // ノードを取得する
            var node = GetNode(x, y);
            if (node.IsNone() == false)
            {
                // 既にOpenしているので何もしない
                return null;
            }

            // Openする
            node.Open(parent, cost);
            AddOpenList(node);

            return node;
        }

        // 周りをOpenする
        public void OpenAround(A_StarNode parent, bool isAround)
        {
            var xbase = parent.X; // 基準座標(X)
            var ybase = parent.Y; // 基準座標(Y)
            var cost = parent.Cost; // コスト
            cost += 1; // 一歩進むので+1する
            // 4方向を開く
            var x = xbase;
            var y = ybase;
            OpenNode(x - 1, y, cost, parent); // 右
            OpenNode(x, y - 1, cost, parent); // 上
            OpenNode(x + 1, y, cost, parent); // 左
            OpenNode(x, y + 1, cost, parent); // 下

            if (isAround)
            {
                OpenNode(x - 1, y - 1, cost, parent); // 右
                OpenNode(x + 1, y - 1, cost, parent); // 上
                OpenNode(x + 1, y + 1, cost, parent); // 左
                OpenNode(x - 1, y + 1, cost, parent); // 下
            }
        }

        // 最小スコアのノードを取得する
        public A_StarNode SearchMinScoreNodeFromOpenList()
        {
            // 最小スコア
            int min = 9999;
            // 最小実コスト
            int minCost = 9999;
            A_StarNode minNode = null;
            foreach (A_StarNode node in openList)
            {
                int score = node.GetGeneralCost();
                if (score > min)
                {
                    // スコアが大きい
                    continue;
                }
                if (score == min && node.Cost >= minCost)
                {
                    // スコアが同じときは実コストも比較する
                    continue;
                }
                
                    
                // 最小値更新
                min = score;
                minCost = node.Cost;
                minNode = node;
            }
            return minNode;
        }
    }

    /// <summary>
    /// 重いほう
    /// </summary>
    /// <returns></returns>
    public Point A_StarProc_Single2(bool isAround)
    {
        if (startPoint.x == goalPoint.x &&
        startPoint.y == goalPoint.y)
        {
            IsArrival = true;
        }
        if (!IsArrival)
        {
            List<Point> pointList = new List<Point>();
            // A-star実行.
            {
                // スタート地点のノード取得
                // スタート地点なのでコストは「0」
                nodeMGR.ClearOpenList();
                A_StarNode node = nodeMGR.OpenNode(startPoint.x, startPoint.y, 0, null);
                nodeMGR.AddOpenList(node);

                // 試行回数。1000回超えたら強制中断
                int cnt = 0;
                while (cnt < 1000)
                {
                    nodeMGR.RemoveOpenList(node);
                    // 周囲を開く
                    nodeMGR.OpenAround(node, isAround);
                    // 最小スコアのノードを探す
                    node = nodeMGR.SearchMinScoreNodeFromOpenList();
                    if (node == null)
                    {
                        // 袋小路なのでおしまい
                        AdDebug.Log("囲", true);
                        return StartPoint;
                    }
                    if (node.X == goalPoint.x && node.Y == goalPoint.y)
                    {
                        // ゴールにたどり着いた
                        nodeMGR.RemoveOpenList(node);
                        // パスを取得する
                        node.GetPath(pointList);
                        // 反転する
                        pointList.Reverse();

                        nodeMGR.Reset();

                        startPoint = pointList[1];

                        return pointList[1];
                    }

                    // カウントアップ
                    ++cnt;
                }
            }
            // 発見できなかった場合
            AdDebug.Log("経路探索失敗", Color.red, 20, true);
            
            return StartPoint;
        }
        return StartPoint;
    }

    // 一回分の移動
    /// <summary>
    /// 軽いほう
    /// </summary>
    /// <returns></returns>
    public Point A_StarProc_Single(bool isAround)
    {
        if (startPoint.x == goalPoint.x &&
            startPoint.y == goalPoint.y)
        {
            IsArrival = true;
        }
        if (!IsArrival)
        {
            List<Point> pointList = new List<Point>();
            // A-star実行.
            // スタート地点のノード取得
            // スタート地点なのでコストは「0」
            nodeMGR.ClearOpenList();
            A_StarNode node = nodeMGR.OpenNode(startPoint.x, startPoint.y, 0, null);

            nodeMGR.RemoveOpenList(node);
            // 周囲を開く
            nodeMGR.OpenAround(node, isAround);
            // 最小スコアのノードを探す
            node = nodeMGR.SearchMinScoreNodeFromOpenList();
            if (node == null)
            {
                // 袋小路なのでおしまい
                AdDebug.Log("囲", true);
                return StartPoint;
            }
            nodeMGR.RemoveOpenList(node);
            // パスを取得する
            node.GetPath(pointList);

            //AdDebug.Log("経路探索終了", Color.cyan, 20, true);

            nodeMGR.Reset();

            startPoint = node.GetPoint();

            return node.GetPoint();
        }
        else
        {
            // すでに到着していた場合
            AdDebug.Log("到着してるので動きません", Color.yellow, 20, true);
            return StartPoint;
        }
    }

    public Point A_StarProc_Specified(int num, bool isAround)
    {
        if (startPoint.x == goalPoint.x &&
            startPoint.y == goalPoint.y)
        {
            IsArrival = true;
        }
        if (!IsArrival)
        {
            List<Point> pointList = new List<Point>();
            // A-star実行
            {
                // スタート地点のノード取得
                // スタート地点なのでコストは「0」
                nodeMGR.ClearOpenList();
                A_StarNode node = nodeMGR.OpenNode(startPoint.x, startPoint.y, 0, null);
                nodeMGR.AddOpenList(node);

                int cnt = 0;
                while (cnt < 3) // 3回実行
                {
                    nodeMGR.RemoveOpenList(node);
                    // 周囲を開く
                    nodeMGR.OpenAround(node, isAround);
                    // 最小スコアのノードを探す
                    node = nodeMGR.SearchMinScoreNodeFromOpenList();
                    if (node == null)
                    {
                        // 袋小路なのでおしまい
                        AdDebug.Log("囲", true);
                        return StartPoint;
                    }
                    // カウントアップ
                    ++cnt;
                }

                nodeMGR.RemoveOpenList(node);
                // パスを取得する
                node.GetPath(pointList);
                // 反転する
                pointList.Reverse();

                //AdDebug.Log("経路探索終了", Color.cyan, 20, true);

                nodeMGR.Reset();

                if (num < pointList.Count)
                {
                    startPoint = pointList[num];

                    return pointList[num];
                }
                else
                {
                    this.A_StarProc_Single(isAround);
                }
            }
        }
        return StartPoint;
    }

    public Point SimpleProc(Point point)
    {
        if (startPoint.x == goalPoint.x &&
            startPoint.y == goalPoint.y)
        {
            IsArrival = true;
        }
        if (!IsArrival)
        {
            // プレイヤーへの距離を求める
            int dx = point.x - goalPoint.x;
            int dy = point.y - goalPoint.y;

            Point startPointCpy = startPoint;

            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                // X方向への距離の方が遠いのでそっちに進む
                if (dx < 0)
                {
                    startPointCpy = new Point(point.x + 1, point.y);
                    if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
                } // 左
                else
                {
                    startPointCpy = new Point(point.x - 1, point.y);
                    if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
                } // 右
            }
            else
            {
                // Y方向へ進む
                if (dy < 0)
                {
                    startPointCpy = new Point(point.x, point.y + 1);
                    if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
                } // 上
                else
                {
                    startPointCpy = new Point(point.x, point.y - 1);
                    if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
                } // 下
            }

            

            // 2重に検査することで壁で詰まることをなくす
            if (dx < 0)
            {
                startPointCpy = new Point(point.x + 1, point.y);
                if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
            } // 左
            else
            {
                startPointCpy = new Point(point.x - 1, point.y);
                if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
            } // 右
            if (dy < 0)
            {
                startPointCpy = new Point(point.x, point.y + 1);
                if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
            } // 上
            else
            {
                startPointCpy = new Point(point.x, point.y - 1);
                if (this.CheckWall_StartPointSet(startPointCpy)) return startPointCpy;
            } // 下
        }
        return startPoint;
    }

    /// <summary>
    /// 指定したグリッド座標が壁かを判断して、壁でないなら
    /// Astarのスタート地点を指定したグリッド座標にする
    /// </summary>
    /// <param name="point">壁か判定するグリッド座標</param>
    public bool CheckWall_StartPointSet(Point point)
    {
        if (MapData.instance.GetMapChipType(point) != MapData.MapChipType.wall)
        {
            startPoint = point;
            return true;
        }
        return false;
    }

    public void SetGoal(Point point)
    {
        goalPoint = point;
        nodeMGR.SetGoal = goalPoint;
        IsArrival = false;
    }

}
