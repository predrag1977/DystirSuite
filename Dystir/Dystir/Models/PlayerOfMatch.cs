using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dystir.Models
{
    public class PlayerOfMatch
    {
        [JsonProperty("PlayerOfMatchID")]
        public int PlayerOfMatchID { get; set; }
        
        string _firstName;
        [JsonProperty("FirstName")]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        string _lastName;
        [JsonProperty("Lastname")]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        [JsonProperty("TeamName")]
        public string TeamName { get; internal set; }

        int? _number;
        [JsonProperty("Number")]
        public int? Number
        {
            get { return _number; }
            set { _number = value;  }
        }

        int? _playingStatus;
        [JsonProperty("PlayingStatus")]
        public int? PlayingStatus {
            get { return _playingStatus; }
            set { _playingStatus = value;  }
        }

        string _position;
        [JsonProperty("Position")]
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [JsonProperty("PlayerID")]
        public int? PlayerID { get; set; }

        [JsonProperty("MatchID")]
        public int? MatchID { get; set; }

        [JsonProperty("TeamID")]
        public int? TeamID { get; set; }

        [JsonProperty("MatchTypeID")]
        public int? MatchTypeID { get; set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; internal set; }

        [JsonProperty("Assist")]
        public int? Assist { get; internal set; }

        int? goal;
        [JsonProperty("Goal")]
        public int? Goal
        {
            get { return goal; }
            set { goal = value; GoalVisible = value != null && value > 0; }
        }

        int? ownGoal;
        [JsonProperty("OwnGoal")]
        public int? OwnGoal
        {
            get { return ownGoal; }
            set { ownGoal = value; OwnGoalVisible = value != null && value > 0; }
        }

        int? yellowCard;
        [JsonProperty("YellowCard")]
        public int? YellowCard
        {
            get { return yellowCard; }
            set {
                yellowCard = value;
                YellowCardVisible = value != null && value > 0;
                SecondYellowCardVisible = value != null && value > 1;
            }
        }

        int? redCard;
        [JsonProperty("RedCard")]
        public int? RedCard
        {
            get { return redCard; }
            set
            {
                redCard = value;
                RedCardVisible = (value != null && value > 0) || SecondYellowCardVisible == true;
            }
        }

        int? subIn;
        [JsonProperty("SubIn")]
        public int? SubIn
        {
            get { return subIn; }
            set
            {
                subIn = value;
                SubInVisible = (value != null && value >= 0);
            }
        }

        int? subOut;
        [JsonProperty("SubOut")]
        public int? SubOut
        {
            get { return subOut; }
            set
            {
                subOut = value;
                SubOutVisible = (value != null && value >= 0);
            }
        }

        public bool GoalVisible { get; internal set; }

        public bool OwnGoalVisible { get; internal set; }

        public bool YellowCardVisible { get; internal set; }

        public bool SecondYellowCardVisible { get; internal set; }

        public bool RedCardVisible { get; internal set; }

        public bool SubInVisible { get; internal set; }

        public bool SubOutVisible { get; internal set; }

        public string NumberOrder { get; internal set; }
    }
}