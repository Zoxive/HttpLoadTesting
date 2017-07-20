import * as React from "react";
import { connect } from "react-redux";

interface LineGraphProps
{
}

interface LineGraphConnectedProps extends LineGraphProps
{
}

export function LineGraph(props: LineGraphConnectedProps)
{
    return (
        <div>
            <h1>LineGraph</h1>
        </div>
    );
}

function mapStateToProps(state: any, props: LineGraphProps)
{
    return {};
}

export default connect(mapStateToProps)(LineGraph);