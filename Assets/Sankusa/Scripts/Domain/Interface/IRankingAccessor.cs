using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Sankusa.unity1week202209.Domain {
    public interface IRankingAccessor
    {
        UniTask<bool> SaveRecord(RankingRecord record, CancellationToken cancellationToken);
        UniTask<bool> UpdateName(string key, string name, CancellationToken cancellationToken);
        UniTask<bool> UpdateStructure(string key, SquareStructure structure, CancellationToken cancellationToken);
        UniTask<RankingRecord> FetchRecord(string key, CancellationToken cancellationToken);
        UniTask<int> FetchRank(long Score, CancellationToken cancellationToken);
        UniTask<Tuple<int, List<RankingRecord>>> FetchNeighbors(int centerRank, int greaterRecordNum, int lessRecordNum, CancellationToken cancellationToken);
        UniTask<bool> IncrementScore(string key, long increment, CancellationToken cancellationToken);
        UniTask<bool> AddBattleHistory(string key, bool sendBattle, string enemyName, int result, long scoreIncrement, CancellationToken cancellationToken);
        UniTask<List<Tuple<bool, string, int, long>>> FetchBattleHistory(string key, CancellationToken cancellationToken);
        // まとめて更新用(リクエスト数削減のため)
        UniTask<bool> IncrementScoreAndAndAddBattleHistory(string key, bool sendBattle, string enemyName, int result, long scoreIncrement, CancellationToken cancellationToken);

    }
}