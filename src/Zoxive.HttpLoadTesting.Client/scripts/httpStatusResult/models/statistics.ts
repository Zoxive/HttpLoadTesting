export type Statistics =
{
    method: string;
    requestUrl: string;
    averageDuration: number;
    durationCount: number;
    stdDev: number;
    numberOfStdDevs: number;
    averageDurationWithinStdDevs: number;
    durationWithinStdDevsCount: number;
}

export default Statistics;