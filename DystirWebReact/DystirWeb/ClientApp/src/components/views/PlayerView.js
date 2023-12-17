import React, { Component } from 'react';

export class PlayerView extends Component {
    static displayName = PlayerView.name;

    constructor(props) {
        super(props);
    }

    render() {
        const player = this.props.player;
        let contents =
            <div className="player-list-item">
                <table className="content_table" style={{ textAlign: "left" }}>
                    <tbody>
                        <tr>
                            <td style={{ textAlign: "center" }, { verticalAlign: "central" }, { width: "35px" }}>
                                <div className="player_number">{player.number}</div>
                            </td>
                            <td>
                                <div className="player_name">
                                    {((player.firstName ?? "") + " " + (player.lastname ?? "")).trim()}
                                </div>
                                <div style={{ color: "#a6a6a6" }}>
                                    <span style={{ color: "#a6a6a6" }, { marginRight: "4px" }}>
                                        {/*@if (Player.Position == "GK")*/}
                                        {/*{*/}
                                        {/*    @("MM")*/}
                                        {/*}*/}
                                        {/*            else if (Player.Position == "DEF")*/}
                                        {/*            {*/}
                                        {/*    @("VL")*/}
                                        {/*}*/}
                                        {/*            else if (Player.Position == "MID")*/}
                                        {/*            {*/}
                                        {/*    @("MV")*/}
                                        {/*}*/}
                                        {/*            else if (Player.Position == "ATT")*/}
                                        {/*            {*/}
                                        {/*    @("AL")*/}
                                        {/*}*/}
                                        {/*            else*/}
                                        {/*            {*/}
                                        {/*    @("---")*/}
                                        {/*}*/}
                                    </span>
                                    {/*@if (Player.Goal > 0)*/}
                                    {/*{*/}
                                    {/*    <span className="goal_icon">&#9917;</span>*/}
                                    {/*    <span>@(Player.Goal)</span>*/}
                                    {/*}*/}
                                    {/*@if (Player.OwnGoal > 0)*/}
                                    {/*            {*/}
                                    {/*    <span className="owngoal_icon">&#9917;</span>*/}
                                    {/*    <span>@(Player.OwnGoal)</span>*/}
                                    {/*            }*/}
                                    {/*@if (Player.YellowCard > 0)*/}
                                    {/*            {*/}
                                    {/*                <span className="yellow_card" style="margin-right:2px;"></span>*/}
                                    {/*            }*/}
                                    {/*@if (Player.YellowCard > 1)*/}
                                    {/*            {*/}
                                    {/*                <span className="yellow_card" style="margin-right:2px;"></span>*/}
                                    {/*            }*/}
                                    {/*@if (Player.RedCard > 0)*/}
                                    {/*            {*/}
                                    {/*                <span className="red_card" style="margin-right:2px;"></span>*/}
                                    {/*            }*/}
                                    {/*@if (Player.SubIn > -1)*/}
                                    {/*            {*/}
                                    {/*    <span className="sub_in">&#9650;</span>*/}
                                    {/*    <span>@(Player.SubIn)'</span>*/}
                                    {/*            }*/}
                                    {/*@if (Player.SubOut > -1)*/}
                                    {/*            {*/}
                                    {/*    <span className="sub_out">&#9660;</span>*/}
                                    {/*    <span>@(Player.SubOut)'</span>*/}
                                    {/*}*/}
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

        return contents
    }
}