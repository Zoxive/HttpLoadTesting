import { Dispatch } from "redux";
import State from "../../store/state";
import GraphStat from "../model/graphstat";

export const RECIEVE_GRAPH_STATS = "RECIEVE_GRAPH_STATS";

export interface IActionGraphStats extends IAction
{
    groupSize: number;
    graphStats: GraphStat[];
}

export function fetchGraphStats(groupSize: number)
{
    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        var url = `/api/graphstats/get/` + groupSize;

        return fetch(url)
            .then(response => response.json())
            .then((json:any) => dispatch(recieveGraphStats(json, groupSize)));
    };
}

export function recieveGraphStats(json: GraphStat[], groupSize: number): IActionGraphStats
{
    return {
        type: RECIEVE_GRAPH_STATS,
        graphStats: json,
        groupSize: groupSize
    };
}