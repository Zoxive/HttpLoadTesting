import * as React from "react";
import StatisticsState from "./../store/statisticsState";
import { connect, Dispatch } from "react-redux";
import { Statistics } from "./../models/statistics";


type StatisticProps =
{
    methods?: string[],
    requestUrls?: string[],
    numberOfDeviations?: number,
    statistics?: Statistics,
    dispatch?: Dispatch<StatisticsState>
};

export function HttpStatusResultStatistics(props: StatisticProps) {
    console.log("props", props);
    var methodOptions = props.methods.map(function(option: any) {
        return (
            <option key={option} value={option}>
                {option}
            </option>
        )
    });

    var requestUrlOptions = props.requestUrls.map(function(option: any) {
        return (
            <option key={option} value={option}>
                {option}
            </option>
        )
    });

    var methodChanged = function(e: any)
    {
        if(e.target.value === this.props.statistics.method)
        {
            return;
        }

        console.log("fetch statistics for method "  + e.target.Value);
    };

    var requestUrlChanged = function(e: any)
    {
        if(e.target.value === this.props.statistics.requestUrl)
        {
            return;
        }

        console.log("fetch statistics for method "  + e.target.Value);
    };

    var numberOfStdDevsChanged = function(e: any)
    {
        if(e.target.value === this.props.numberOfDeviations)
        {
            return;
        }

        console.log("fetch statistics for numberOfDeviations "  + e.target.Value);
    };
    
    return (
        <div>
            <div>--------Statistics Calculation Options--------</div>
            <div>Method: 
                <select value={props.statistics.method} onChange={methodChanged}>
                    <option key='' value='' />
                    {methodOptions}
                </select>
            </div>
            <div>Request Url: 
                <select value={props.statistics.requestUrl} onChange={requestUrlChanged}>
                    <option key='' value='' />
                    {requestUrlOptions}
                </select>
            </div>
            <div>Number of Std Devs: 
                <input value={props.numberOfDeviations} onChange={numberOfStdDevsChanged}>
                </input>
            </div>
            <br/>
            <div>--------Result Statistics--------</div>
            <div>Average Duration: {props.statistics.averageDuration} ms</div>
            <div>Number of Requests: {props.statistics.durationCount}</div>
            <br/>
            <div>Standard Deviation: {props.statistics.standardDeviation} ms</div>
            <br/>
            <div>Average Duration of Requests within {props.numberOfDeviations} StdDevs of Avg: {props.statistics.averageDurationWithinDeviations} ms</div>
            <div>Number of Requests within {props.numberOfDeviations} StdDevs of Avg: {props.statistics.durationWithinDeviationsCount} </div>
        </div>
    );
}

function mapStateToProps(state: any, props: StatisticProps): StatisticProps {
    return {
        methods: state.statistics.methods,
        requestUrls: state.statistics.requestUrls,
        numberOfDeviations: state.statistics.numberOfDeviations,
        statistics: state.statistics.statistics
    };
}

export default connect(mapStateToProps)(HttpStatusResultStatistics);