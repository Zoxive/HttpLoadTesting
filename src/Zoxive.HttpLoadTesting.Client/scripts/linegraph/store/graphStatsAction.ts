import { Dispatch } from "redux";
import State from "../../store/state";
import GraphStat from "../model/graphstat";

export const RECIEVE_GRAPH_STATS = "RECIEVE_GRAPH_STATS";

export interface IActionGraphStats extends IAction
{
    graphStats: GraphStat[]
}

export function fetchGraphStats(groupSize: number)
{
    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        var url = `/api/graphstats/get/` + groupSize;
        return fetch(url)
            .then(response => response.json())
            .then(json => dispatch(recieveGraphStats(json)));
    };
}

export function recieveGraphStats(json: GraphStat[]): IActionGraphStats
{
    return {
        type: RECIEVE_GRAPH_STATS,
        graphStats: json
    };
}