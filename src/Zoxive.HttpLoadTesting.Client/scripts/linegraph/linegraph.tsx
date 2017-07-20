import * as React from "react";
import { connect } from "react-redux";
import State from "../store/state";
import GraphStat from "./model/graphstat";
import { fetchGraphStats } from "./store/graphStatsAction";
import { LineChart, Line, Legend, XAxis, YAxis, Tooltip } from "recharts";

export function LineGraph(props: ILineGraphConnectedProps & ILineGraphDispatchProps)
{
    if (props.groupSize === 0)
    {
        props.fetchGraphStats(60);
    }

    return (
        <div>
            <h1>LineGraph</h1>

            <div>
                <LineChart width={1600} height={800} data={props.stats} >
                    <XAxis/>
                    <YAxis />
                    <YAxis yAxisId="right" orientation="right" />
                    <Tooltip viewBox={{ x: 0, y: 0, width: 1600, height: 800 }} />
                    <Legend chartWidth={1600} chartHeight={800} />
                    <Line type="monotone" dataKey="requests" points={null} name="Requests" unit=" count" dot={{ stroke: 'red'}} /* yAxisId="right" */ />
                    <Line type="monotone" dataKey="avg" points={null} name="AVG" unit="ms" />
                    <Line type="monotone" dataKey="min" points={null} name="MIN" unit="ms" />
                    <Line type="monotone" dataKey="max" points={null} name="MAX" unit="ms" />
                    <Line type="monotone" dataKey="std" points={null} name="STD" unit="ms" />
                </LineChart>
            </div>
        </div>
    );
}

interface LineGraphProps
{
}

interface ILineGraphConnectedProps extends LineGraphProps
{
    groupSize: number;
    stats: GraphStat[];
}

interface ILineGraphDispatchProps
{
    fetchGraphStats(groupSize: number): void;
}

function mapStateToProps(state: State, props: LineGraphProps): ILineGraphConnectedProps
{
    return {
        groupSize: state.graphStats.groupSize,
        stats: state.graphStats.stats
    };
}

function mapDispatchToProps(dispatch: any): ILineGraphDispatchProps
{
    return {
        fetchGraphStats: (groupSize: number) => dispatch(fetchGraphStats(groupSize))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(LineGraph);