using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sankusa.unity1week202209.Domain;
using Cysharp.Threading.Tasks;
using System.Threading;
using NCMB;
using UnityEngine.Events;
using System;

namespace Sankusa.unity1week202209.DataAccess {
    public class NCMBRankingAccessor : IRankingAccessor
    {
        private const string className = "Record";
        private const string columnName_Name = "Name";
        private const string columnName_Score = "Score";
        private const string columnName_Structure = "Structure";
        private const string columnName_BattleHistory = "BattleHistory";

        // 初回アップロード用
        public async UniTask<bool> SaveRecord(RankingRecord record, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            bool finished = false;
            NCMBObject obj = new NCMBObject(className);
            if(record.Key != "") obj.ObjectId = record.Key;
            obj[columnName_Name] = record.Name;
            obj[columnName_Score] = record.Score;
            obj[columnName_Structure] = JsonUtility.ToJson(record.Structure);
            obj[columnName_BattleHistory] = new string[]{};
            obj.SaveAsync((NCMBException e) => {
                if(e != null) {

                } else {
                    record.Key = obj.ObjectId;
                    success = true;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return success;
        }

        public async UniTask<bool> UpdateName(string key, string name, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            bool finished = false;
            NCMBObject obj = new NCMBObject(className);
            obj.ObjectId = key;
            obj[columnName_Name] = name;
            obj.SaveAsync((NCMBException e) => {
                if(e != null) {

                } else {
                    success = true;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return success;
        }

        public async UniTask<bool> UpdateStructure(string key, SquareStructure structure, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            bool finished = false;
            NCMBObject obj = new NCMBObject(className);
            obj.ObjectId = key;
            obj[columnName_Structure] = JsonUtility.ToJson(structure);
            obj.SaveAsync((NCMBException e) => {
                if(e != null) {

                } else {
                    success = true;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return success;
        }

        // 1件取得
        public async UniTask<RankingRecord> FetchRecord(string key, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(className);
            bool finished = false;
            NCMBObject obj = null;
            query.GetAsync(key, (NCMBObject record, NCMBException e) => {
                if(e != null) {

                } else {
                    obj = record;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            if(obj == null) {
                return null;
            } else {
                return new RankingRecord(key,
                                         Convert.ToString(obj[columnName_Name]),
                                         Convert.ToInt64(obj[columnName_Score]),
                                         JsonUtility.FromJson<SquareStructure>(Convert.ToString(obj[columnName_Structure])));
            }
        }

        // 入力スコアが何位か
        public async UniTask<int> FetchRank(long Score, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(className);
            bool finished = false;
            int rank = 0;
            query.WhereGreaterThan(columnName_Score, Score);
            query.CountAsync((int count, NCMBException e) => {
                if(e != null) {

                } else {
                    rank = count + 1;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return rank;
        }

        /// <summury>
        /// <returns> 入力されたランク周辺のレコードリストを取得、intはレコードリスト内最高順位 </returns>
        /// </summury>
        public async UniTask<Tuple<int, List<RankingRecord>>> FetchNeighbors(int centerRank, int greaterRecordNum, int lessRecordNum, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool finished = false;

            List<RankingRecord> records = new List<RankingRecord>();
            int skipNum = centerRank - greaterRecordNum - 1;
            if(skipNum < 0) skipNum = 0;

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(className);
            query.OrderByDescending(columnName_Score);
            query.Skip = skipNum;
            query.Limit = greaterRecordNum + 1 + lessRecordNum;
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if(e != null) {

                } else {
                    foreach(NCMBObject obj in objList) {
                        records.Add(
                            new RankingRecord(
                                obj.ObjectId,
                                Convert.ToString(obj[columnName_Name]),
                                Convert.ToInt64(obj[columnName_Score]),
                                JsonUtility.FromJson<SquareStructure>(Convert.ToString(obj[columnName_Structure]))
                            )
                        );
                    }
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return new Tuple<int, List<RankingRecord>>(skipNum + 1, records);
        }

        public async UniTask<bool> IncrementScore(string key, long increment, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            bool finished = false;
            NCMBObject obj = new NCMBObject(className);
            obj.ObjectId = key;
            obj.Increment(columnName_Score, increment);
            obj.SaveAsync((NCMBException e) => {
                if(e != null) {

                } else {
                    success = true;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return success;
        }

        public async UniTask<bool> AddBattleHistory(string key, bool sendBattle, string enemyName, int result, long scoreIncrement, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            bool finished = false;
            NCMBObject obj = new NCMBObject(className);
            obj.ObjectId = key;
            obj.AddToList(columnName_BattleHistory, sendBattle.ToString() + "," + enemyName + "," + result + "," + scoreIncrement);
            obj.SaveAsync((NCMBException e) => {
                if(e != null) {

                } else {
                    success = true;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return success;
        }

        public async UniTask<List<Tuple<bool, string, int, long>>> FetchBattleHistory(string key, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            List<Tuple<bool, string, int, long>> histories = new List<Tuple<bool, string, int, long>>();

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(className);
            bool finished = false;
            NCMBObject obj = null;
            query.GetAsync(key, (NCMBObject record, NCMBException e) => {
                if(e != null) {

                } else {
                    obj = record;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            if(obj != null) {
                ArrayList array = obj[columnName_BattleHistory] as ArrayList;
                foreach(string history in array) {
                    string[] columns = history.Split(',');
                    histories.Add(new Tuple<bool, string, int, long>(Convert.ToBoolean(columns[0]), Convert.ToString(columns[1]), Convert.ToInt32(columns[2]), Convert.ToInt64(columns[3])));
                }
            }
            return histories;
        }

        // 通信回数削減のため2つ同時に更新
        public async UniTask<bool> IncrementScoreAndAndAddBattleHistory(string key, bool sendBattle, string enemyName, int result, long scoreIncrement, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            bool finished = false;
            NCMBObject obj = new NCMBObject(className);
            obj.ObjectId = key;
            obj.Increment(columnName_Score, scoreIncrement);
            obj.AddToList(columnName_BattleHistory, sendBattle.ToString() + "," + enemyName + "," + result + "," + scoreIncrement);
            obj.SaveAsync((NCMBException e) => {
                if(e != null) {

                } else {
                    success = true;
                }
                finished = true;
            });
            await UniTask.WaitUntil(() => finished == true, cancellationToken: cancellationToken);
            return success;
        }
    }
}