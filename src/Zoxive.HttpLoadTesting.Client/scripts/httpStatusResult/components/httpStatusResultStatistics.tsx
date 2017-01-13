import * as React from "react";
import StatisticsState from "./../store/statisticsState";
import { connect, Dispatch } from "react-redux";
import { Statistics } from "./../models/statistics";

import { fetchStatistics } from "./../store/statisticsAction";
import { fetchMethods } from "./../store/getRequestMethodsAction";
import { fetchRequestUrls } from "./../store/getRequestUrlsAction";

type StatisticProps =
{
    methods?: string[],
    requestUrls?: string[],
    statistics?: Statistics,
    dispatch?: Dispatch<StatisticsState>
};

export function HttpStatusResultStatistics(props: StatisticProps) {
    console.log("props", props, "this", this);
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
        if(e.target.value === props.statistics.method)
        {
            return;
        }

        props.statistics.method = e.target.value;

        props.dispatch(fetchRequestUrls(props.statistics.method));
        props.dispatch(fetchStatistics(props.statistics.method, props.statistics.requestUrl, props.statistics.numberOfStandardDeviations));
    };

    var requestUrlChanged = function(e: any)
    {
        if(e.target.value === props.statistics.requestUrl)
        {
            return;
        }

        props.statistics.requestUrl = e.target.value;

        props.dispatch(fetchMethods(props.statistics.requestUrl));
        props.dispatch(fetchStatistics(props.statistics.method, props.statistics.requestUrl, props.statistics.numberOfStandardDeviations));
    };

    var numberOfStdDevsChanged = function(e: any)
    {
        if(e.target.value === props.statistics.numberOfStandardDeviations)
        {
            return;
        }

        props.statistics.numberOfStandardDeviations = e.target.value;

        props.dispatch(fetchStatistics(props.statistics.method, props.statistics.requestUrl, props.statistics.numberOfStandardDeviations));
    };

    var requestsOutsideOfDeviations = props.statistics.durationCount - props.statistics.durationWithinDeviationsCount;
    
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
                <input type='text' value={props.statistics.numberOfStandardDeviations} onChange={numberOfStdDevsChanged}>
                </input>
            </div>
            <br/>
            <div>--------Result Statistics--------</div>
            <div>Average Duration: {props.statistics.averageDuration} ms</div>
            <div>Number of Requests: {props.statistics.durationCount}</div>
            <br/>
            <div>Standard Deviation: {props.statistics.standardDeviation} ms</div>
            <br/>
            <div>Average Duration of Requests within {props.statistics.numberOfStandardDeviations} Std Devs of Avg: {props.statistics.averageDurationWithinDeviations} ms</div>
            <div>Number of Requests within {props.statistics.numberOfStandardDeviations} Std Devs of Avg: {props.statistics.durationWithinDeviationsCount} </div>
            <div>Number of Requests outside of {props.statistics.numberOfStandardDeviations} Std Devs from Avg: {requestsOutsideOfDeviations} </div>
        </div>
    );
}

function mapStateToProps(state: any, props: StatisticProps): StatisticProps {
    return {
        methods: state.statistics.methods,
        requestUrls: state.statistics.requestUrls,
        statistics: state.statistics.statistics
    };
}

export default connect(mapStateToProps)(HttpStatusResultStatistics);