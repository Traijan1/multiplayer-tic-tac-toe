using Model;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend;

public class Game {
    public TicTacToe Match { get; private set; }
    public List<string> Users { get; private set; }

    Guid id;
    public string Id { get { return id.ToString("N"); } }

    public string MatchAsJson { get { return JsonConvert.SerializeObject(Match.Field); } }
    public List<string> AllowNewRound { get; private set; }

    public Game() {
        Match = new();
        Users = new();
        id = Guid.NewGuid();
        AllowNewRound = new();
    }

    public bool AddUser(string connection) {
        if (Users.Count >= 2)
            return false;

        Users.Add(connection);
        return true;
    }

    public bool UserAllowedNewRound(string callerId) {
        if (AllowNewRound.Count == 2)
            return true;

        if (!AllowNewRound.Exists(id => id == callerId))
            AllowNewRound.Add(callerId);

        return AllowNewRound.Count == 2;
    }

    public void NewGame() {
        Match = new();
        AllowNewRound.Clear();
    }

    public bool NewMatchState(int x, int y) {
        return Match.SetChar(x, y);
    } 
}
