using Newtonsoft.Json;

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
        public string TeamName { get; set; }

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
            set { _position = value; IsPositionVisible = (value?.Length ?? 0) > 0; }
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
        public string MatchTypeName { get; set; }

        [JsonProperty("Assist")]
        public int? Assist { get; set; }

        int? goal;
        [JsonProperty("Goal")]
        public int? Goal
        {
            get { return goal; }
            set
            {
                goal = value;
                GoalVisible = value != null && value > 0;
                GoalImageSource = GoalVisible ? "resource://Dystir.Resources.Images.goal.svg" : "";
            }
        }

        int? ownGoal;
        [JsonProperty("OwnGoal")]
        public int? OwnGoal
        {
            get { return ownGoal; }
            set {
                ownGoal = value;
                OwnGoalVisible = value != null && value > 0;
                OwnGoalImageSource = OwnGoalVisible ? "resource://Dystir.Resources.Images.owngoal.svg" : "";
            }
        }

        int? yellowCard;
        [JsonProperty("YellowCard")]
        public int? YellowCard
        {
            get { return yellowCard; }
            set {
                yellowCard = value;
                YellowCardVisible = value != null && value > 0;
                YellowCardImageSource = YellowCardVisible ? "resource://Dystir.Resources.Images.yellow.svg" : "";
                SecondYellowCardVisible = value != null && value > 1;
                SecondYellowCardImageSource = SecondYellowCardVisible ? "resource://Dystir.Resources.Images.yellow.svg" : "";

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
                RedCardImageSource = RedCardVisible ? "resource://Dystir.Resources.Images.red.svg" : "";
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
                SubInImageSource = SubInVisible ? "resource://Dystir.Resources.Images.sub_in.svg" : "";
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
                SubOutImageSource = SubOutVisible ? "resource://Dystir.Resources.Images.sub_out.svg" : "";
            }
        }

        public bool GoalVisible { get; set; } = false;

        public bool OwnGoalVisible { get; set; } = false;

        public bool YellowCardVisible { get; set; } = false;

        public bool SecondYellowCardVisible { get; set; } = false;

        public bool RedCardVisible { get; set; } = false;

        public bool SubInVisible { get; set; } = false;

        public bool SubOutVisible { get; set; } = false;

        public string NumberOrder { get; set; } = string.Empty;

        public string GoalImageSource { get; set; } = string.Empty;

        public string OwnGoalImageSource { get; set; } = string.Empty;

        public string YellowCardImageSource { get; set; } = string.Empty;

        public string SecondYellowCardImageSource { get; set; } = string.Empty;

        public string RedCardImageSource { get; set; } = string.Empty;

        public string SubInImageSource { get; set; } = string.Empty;

        public string SubOutImageSource { get; set; } = string.Empty;

        public bool IsPositionVisible { get; set; }
    }
}