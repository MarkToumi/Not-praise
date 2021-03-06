/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using live2d;

namespace live2d.framework
{
    /*
     * 差分モーション。
     * 通常のモーションは値をsetParamFloatでセットするが、
     * この差分モーションでは値を足すか、掛けるかする。
     *
     * Live2DライブラリのAMotionを継承しているのでMotionQueueManagerで管理できる。
     */
    public class L2DExpressionMotion : AMotion
    {
        private const string EXPRESSION_DEFAULT = "DEFAULT";// 表情のデフォルト値要素のキー

        public const int TYPE_SET = 0;
        public const int TYPE_ADD = 1;
        public const int TYPE_MULT = 2;

        private List<L2DExpressionParam> paramList;

        /*
         * コンストラクタ
         */
        public L2DExpressionMotion()
        {
            paramList = new List<L2DExpressionParam>();
        }


        /*
         * モデルのパラメータを更新する。
         * 引数の詳細はドキュメントを参照。
         */
        public override void updateParamExe(ALive2DModel model, long timeMSec, float weight, MotionQueueEnt motionQueueEnt)
        {
            for (int i = paramList.Count - 1; i >= 0; --i)
            {
                L2DExpressionParam param = paramList[i];
                if (param.type == TYPE_ADD)
                {
                    model.addToParamFloat(param.id, param.value, weight);// 相対変化 加算
                }
                else if (param.type == TYPE_MULT)
                {
                    model.multParamFloat(param.id, param.value, weight);// 相対変化 乗算
                }
                else if (param.type == TYPE_SET)
                {
                    model.setParamFloat(param.id, param.value, weight);// 絶対変化
                }
            }
        }

        public static L2DExpressionMotion loadJson(byte[] buf)
        {
            return loadJson(System.Text.Encoding.GetEncoding("UTF-8").GetString(buf));
        }


        public static L2DExpressionMotion loadJson(string buf)
        {
            return loadJson(buf.ToCharArray());
        }

        /*
         * JSONファイルから読み込み。
         * 仕様についてはマニュアル参照。JSONスキーマの形式の仕様がある。
         * @param buf
         * @return
         */
        public static L2DExpressionMotion loadJson(char[] buf)
        {
            L2DExpressionMotion ret = new L2DExpressionMotion();

            Value json = Json.parseFromBytes(buf);

            ret.setFadeIn(json.get("fade_in").toInt(1000));// フェードイン
            ret.setFadeOut(json.get("fade_out").toInt(1000));// フェードアウト

            if (!json.getMap(null).ContainsKey("params")) return ret;

            // パラメータ一覧
            Value parameters = json.get("params");
            int paramNum = parameters.getVector(null).Count;

            ret.paramList = new List<L2DExpressionParam>(paramNum);

            for (int i = 0; i < paramNum; i++)
            {
                Value param = parameters.get(i);
                string paramID = param.get("id").toString();// パラメータID
                float value = param.get("val").toFloat();// 値

                // 計算方法の設定
                int calcTypeInt = TYPE_ADD;
                string calc = param.getMap(null).ContainsKey("calc") ? (param.get("calc").toString()) : "add";
                if (calc.Equals("add"))
                {
                    calcTypeInt = TYPE_ADD;
                }
                else if (calc.Equals("mult"))
                {
                    calcTypeInt = TYPE_MULT;
                }
                else if (calc.Equals("set"))
                {
                    calcTypeInt = TYPE_SET;
                }
                else
                {
                    // その他 仕様にない値を設定したときは加算モードにすることで復旧
                    calcTypeInt = TYPE_ADD;
                }

                // 計算方法 加算
                if (calcTypeInt == TYPE_ADD)
                {
                    float defaultValue = (!param.getMap(null).ContainsKey("def")) ? 0 : param.get("def").toFloat();
                    value = value - defaultValue;
                }
                // 計算方法 乗算
                else if (calcTypeInt == TYPE_MULT)
                {
                    float defaultValue = (!param.getMap(null).ContainsKey("def")) ? 1 : param.get("def").toFloat(0);
                    if (defaultValue == 0) defaultValue = 1;// 0(不正値)を指定した場合は1(標準)にする
                    value = value / defaultValue;
                }

                // 設定オブジェクトを作成してリストに追加する
                L2DExpressionParam item = new L2DExpressionParam();

                item.id = paramID;
                item.type = calcTypeInt;
                item.value = value;

                ret.paramList.Add(item);
            }
            return ret;
        }


        /*
         * 旧表情JSONを読み込み
         * @param in
         * @return
         */
        static public Dictionary<string, AMotion> loadExpressionJsonV09(byte[] bytes)
        {
            Dictionary<string, AMotion> expressions = new Dictionary<string, AMotion>();

            char[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetString(bytes).ToCharArray();
            Value mo = Json.parseFromBytes(buf);

            Value defaultExpr = mo.get(EXPRESSION_DEFAULT);// 相対値の基準となる値

            List<string> keys = mo.keySet();
            foreach (string key in keys)
            {
                if (EXPRESSION_DEFAULT.Equals(key)) continue;// 飛ばす

                Value expr = mo.get(key);

                L2DExpressionMotion exMotion = loadJsonV09(defaultExpr, expr);
                expressions.Add(key, exMotion);
            }

            return expressions;// nullには成らない
        }


        /*
         * JSONの解析結果からExpressionを生成する
         * @param v
         */
        static private L2DExpressionMotion loadJsonV09(Value defaultExpr, Value expr)
        {

            L2DExpressionMotion ret = new L2DExpressionMotion();
            ret.setFadeIn(expr.get("FADE_IN").toInt(1000));
            ret.setFadeOut(expr.get("FADE_OUT").toInt(1000));

            // --- IDリストを生成
            Value defaultParams = defaultExpr.get("PARAMS");
            Value parameters = expr.get("PARAMS");
            List<string> paramID = parameters.keySet();
            List<string> idList = new List<string>();

            foreach (string id in paramID)
            {
                idList.Add(id);
            }

            // --------- 値を設定 ---------
            for (int i = idList.Count - 1; i >= 0; --i)
            {
                string id = idList[i];

                float defaultV = defaultParams.get(id).toFloat(0);
                float v = parameters.get(id).toFloat(0.0f);
                float values = (v - defaultV);
                //			ret.addParam(id, value,L2DExpressionMotion.TYPE_ADD);
                L2DExpressionParam param = new L2DExpressionParam();
                param.id = id;
                param.type = L2DExpressionMotion.TYPE_ADD;
                param.value = values;
                ret.paramList.Add(param);
            }

            return ret;
        }


        /*
         * パラメータの設定に使用する
         */
        public class L2DExpressionParam
        {
            public string id;
            //public int index=-1;
            public int type;
            public float value;
        }
    }
}