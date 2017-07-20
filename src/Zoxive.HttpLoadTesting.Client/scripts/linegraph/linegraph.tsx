import * as React from "react";
import { connect } from "react-redux";
import State from "../store/state";
import { fetchGraphStats } from "./store/graphStatsAction";

interface LineGraphProps
{
}

interface LineGraphConnectedProps extends LineGraphProps
{
    groupSize: number;
    fetchGraphStats(groupSize: number): void;
}

export function LineGraph(props: LineGraphConnectedProps)
{
    if (props.groupSize === 0)
    {
        props.fetchGraphStats(60);
    }

    return (
        <div>
            <h1>LineGraph</h1>
        </div>
    );
}

function mapStateToProps(state: State, props: LineGraphProps)
{
    return {
        groupSize: state.graphStats.groupSize
    };
}

function mapDispatchToProps(dispatch: any)
{
    return {
        fetchGraphStats: (groupSize: number) => dispatch(fetchGraphStats(groupSize))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(LineGraph);