import HttpStatusResultStatisticsState from "../httpStatusResult/store/statisticsState";
import { GraphStatsState } from "../linegraph/store/graphStatsReducer";

export type State =
{
    statisticsState: HttpStatusResultStatisticsState;
    graphStats: GraphStatsState;
}

export default State;