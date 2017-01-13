export type Statistics =
{
    method: string;
    requestUrl: string;
    averageDuration: number;
    durationCount: number;
    standardDeviation: number;
    averageDurationWithinDeviations: number;
    durationWithinDeviationsCount: number;
}

export default Statistics;