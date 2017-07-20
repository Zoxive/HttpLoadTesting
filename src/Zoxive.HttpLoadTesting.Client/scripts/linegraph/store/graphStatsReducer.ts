import * as xtend from "xtend";
import GraphStat from "./../model/graphstat";
import { IActionGraphStats, RECIEVE_GRAPH_STATS } from "./graphStatsAction";

export type GraphStatsState =
{
    groupSize: number,
    stats: GraphStat[];
}

export default function graphStatsReducer(state: GraphStatsState, action: IAction): GraphStatsState
{
    if (state == null)
    {
        return {
            groupSize: 0,
            stats: []
        };
    }

    switch (action.type)
    {
        case RECIEVE_GRAPH_STATS:
            const graphStats = action as IActionGraphStats;
            return xtend(state, { stats: graphStats.graphStats });
    }

    return state;
}