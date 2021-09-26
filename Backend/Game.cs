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

    public Game() {
        Match = new();
        Users = new();
        id = Guid.NewGuid();
    }

    public bool AddUser(string connection) {
        if (Users.Count >= 2)
            return false;

        Users.Add(connection);
        return true;
    }

    public bool NewMatchState(int x, int y) {
        return Match.SetChar(x, y);
    } 
}
