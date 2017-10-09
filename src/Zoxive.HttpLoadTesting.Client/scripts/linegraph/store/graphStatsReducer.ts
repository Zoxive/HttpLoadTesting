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
            const { groupSize, graphStats } = action as IActionGraphStats;

            const graphStatsMap = new Map<number, GraphStat>
            (
                graphStats.map(x => [x.grp, x] as [number, GraphStat])
            );

            for (let i = 0; i < groupSize; i++)
            {
                graphStatsMap.set(i, getOrCreateEmptyGraphStat(graphStatsMap.get(i), i));
            }

            // to array and sort by grp
            // TODO make server side add emptys and sort
            const arr = Array.from(graphStatsMap.values())
                .sort((a, b) => a.grp - b.grp);

            return xtend(state, { stats: arr, groupSize: groupSize });
    }

    return state;
}

function getOrCreateEmptyGraphStat(stat: GraphStat, grp: number): GraphStat
{
    if (stat) return stat;

    return {
        grp: grp,
        requests: 0,
        users: 0,
        avg: 0,
        min: 0,
        max: 0,
        variance: 0,
        std: 0
    };
}