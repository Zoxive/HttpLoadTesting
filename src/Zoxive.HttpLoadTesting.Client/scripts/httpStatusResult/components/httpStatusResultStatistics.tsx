import * as React from "react";
import State from "./../../store/state";
import { connect, Dispatch } from "react-redux";
import { Statistics } from "./../models/statistics";


type StatisticProps =
{
    method?: string,
    requestUrl?: string,
    numberOfStdDevs?: number,
    statistics?: Statistics,
    dispatch?: Dispatch<State>
};

export function HttpStatusResultStatistics(props: StatisticProps) {
    return (
        <div className="slide">
            Statistics!
        </div>
    );
}

function mapStateToProps(state: State, props: StatisticProps): StatisticProps {
    return {
        method: state.statistics.method,
        requestUrl: state.statistics.requestUrl,
        numberOfStdDevs: state.statistics.numberOfStdDevs,
        statistics: state.statistics
    };
}
export default connect(mapStateToProps)(HttpStatusResultStatistics);