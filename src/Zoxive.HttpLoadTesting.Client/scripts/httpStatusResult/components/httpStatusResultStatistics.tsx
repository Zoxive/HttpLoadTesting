import * as React from "react";
import StatisticsState from "./../store/statisticsState";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { Statistics } from "./../models/statistics";

import { fetchStatistics } from "./../store/statisticsAction";
import { fetchMethods } from "./../store/getRequestMethodsAction";
import { fetchRequestUrls } from "./../store/getRequestUrlsAction";

interface StatisticProps
{
    methods?: string[];
    requestUrls?: string[];
    statistics?: Statistics;
    fetchRequestUrls?(method: string): void;
    fetchStatistics?(method: string, requestUrl: string, stdDev: number): void;
    fetchMethods?(method: string): void;
};

export function HttpStatusResultStatistics(props: StatisticProps)
{
    var methodOptions = props.methods.map((option: any) =>
    {
        return (
            <option key={option} value={option}>
                {option}
            </option>
        );
    });

    var requestUrlOptions = props.requestUrls.map((option: any) =>
    {
        return (
            <option key={option} value={option}>
                {option}
            </option>
        );
    });

    const statusCodeCounts = props.statistics.statusCodeCounts.map((item: any) =>
    {
        return (
            <tr key={item.statusCode}>
                <td>{item.statusCode} ({item.statusCodeName})</td>
                <td>{item.count}</td>
            </tr>
        );
    });

    const fastestRequestDurations = props.statistics.fastestRequests.map((item: any) =>
    {
        return (
            <tr key={item.id}>
                <td>{item.elapsedMilliseconds}</td>
                <td>{item.method}</td>
                <td>{item.requestUrl}</td>
                <td>{item.statusCode}</td>
            </tr>
        );
    });

    const slowestRequestDurations = props.statistics.slowestRequests.map((item: any) =>
    {
        return (
            <tr key={item.id}>
                <td>{item.elapsedMilliseconds}</td>
                <td>{item.method}</td>
                <td>{item.requestUrl}</td>
                <td>{item.statusCode}</td>
            </tr>
        );
    });

    const methodChanged = (e: any) =>
    {
        if(e.target.value === props.statistics.method)
        {
            return;
        }

        props.statistics.method = e.target.value;

        props.fetchRequestUrls(props.statistics.method);
        props.fetchStatistics(props.statistics.method, props.statistics.requestUrl, props.statistics.numberOfStandardDeviations);
    };

    const requestUrlChanged = (e: any) =>
    {
        if(e.target.value === props.statistics.requestUrl)
        {
            return;
        }

        props.statistics.requestUrl = e.target.value;

        props.fetchMethods(props.statistics.requestUrl);
        props.fetchStatistics(props.statistics.method, props.statistics.requestUrl, props.statistics.numberOfStandardDeviations);
    };

    const numberOfStdDevsChanged = (e: any) =>
    {
        if(e.target.value === props.statistics.numberOfStandardDeviations)
        {
            return;
        }

        props.statistics.numberOfStandardDeviations = e.target.value;

        props.fetchStatistics(props.statistics.method, props.statistics.requestUrl, props.statistics.numberOfStandardDeviations);
    };

    const requestsOutsideOfDeviations = props.statistics.durationCount - props.statistics.durationWithinDeviationsCount;
    const percentageOutsideOfDeviations = (requestsOutsideOfDeviations / props.statistics.durationCount) * 100.0;
    
    return (
        <div>
            <h3>Statistics Calculation Options</h3>
            <div>Method: 
                <select value={props.statistics.method || ""} onChange={methodChanged}>
                    <option key="" value="" />
                    {methodOptions}
                </select>
            </div>
            <div>Request: 
                <select value={props.statistics.requestUrl || ""} onChange={requestUrlChanged}>
                    <option key="" value="" />
                    {requestUrlOptions}
                </select>
            </div>
            <div>Number of Std Devs: 
                <input type="text" value={props.statistics.numberOfStandardDeviations} onChange={numberOfStdDevsChanged}>
                </input>
            </div>
            <br/>
            <h3>Result Statistics</h3>
            <div>Average Duration: {props.statistics.averageDuration} ms</div>
            <div>Number of Requests: {props.statistics.durationCount}</div>
            <br/>
            <div>Standard Deviation: {props.statistics.standardDeviation} ms</div>
            <br/>
            <div>Average Duration of Requests within {props.statistics.numberOfStandardDeviations} Std Devs of Avg: {props.statistics.averageDurationWithinDeviations} ms</div>
            <div>Number of Requests within {props.statistics.numberOfStandardDeviations} Std Devs of Avg: {props.statistics.durationWithinDeviationsCount} </div>
            <br/>
            <div>Number of Requests outside of {props.statistics.numberOfStandardDeviations} Std Devs from Avg: {requestsOutsideOfDeviations} </div>
            <div>Percentage of Requests outside of {props.statistics.numberOfStandardDeviations} Std Devs from Avg: {percentageOutsideOfDeviations}% </div>
            <br/>
            <h3>Status Code Counts</h3>
            <table>
                <thead>
                    <tr>
                        <th>Status Code</th>
                        <th>Count</th>
                    </tr>
                </thead>
                <tbody>
                {statusCodeCounts}
                </tbody>
            </table>
            <br/>
            <h3>Slowest Request Durations</h3>
            <table>
                <thead>
                    <tr>
                        <th>Duration (ms)</th>
                        <th>Method</th>
                        <th>Request Url</th>
                        <th>Status Code</th>
                    </tr>
                </thead>
                <tbody>
                {slowestRequestDurations}
                </tbody>
            </table>
            <br/>
            <h3>Fastest Request Durations</h3>
            <table>
                <thead>
                    <tr>
                        <th>Duration (ms)</th>
                        <th>Method</th>
                        <th>Request Url</th>
                        <th>Status Code</th>
                    </tr>
                </thead>
                <tbody>
                {fastestRequestDurations}
                </tbody>
            </table>
        </div>
    );
}

function mapStateToProps(state: any, props: StatisticProps)
{
    return {
        methods: state.statistics.methods,
        requestUrls: state.statistics.requestUrls,
        statistics: state.statistics.statistics
    };
}

// any sucks here but im running with it for now
function mapDispatchToProps(dispatch: any)
{
    return {
        fetchRequestUrls: (method: string) => dispatch(fetchRequestUrls(method)),
        fetchStatistics: (method: string, requestUrl: string, stdDev: number) => dispatch(fetchStatistics(method, requestUrl, stdDev)),
        fetchMethods: (method: string) => dispatch(fetchMethods(method)),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(HttpStatusResultStatistics);