namespace Bidkee.Client.Internal
{
    internal static class HttpPaths
    {
        public const string Health = "v1/health";
        public const string Gateway = "v1/gateway";
        public const string Search = "v1/search";
        public const string Id = "v1/id/";
        public const string Resolve = "v1/resolve/";
        public const string Verify = "v1/verify";
        public const string VerifyOffline = "v1/verify/offline";
        public const string PresentStart = "v1/present/start";
        public const string PresentComplete = "v1/present/complete";
        public const string SceneCatalog = "v1/issue/scene-catalog";
        public const string Tools = "v1/tools.json";

        public const string AgentCapabilities = "v1/agent/capabilities";
        public const string AgentEnroll = "v1/agent/enroll";
        public const string AgentMe = "v1/agent/me";
        public const string AgentPresentStart = "v1/agent/present/start";
        public const string AgentPresentComplete = "v1/agent/present/complete";
        public const string AgentVerify = "v1/agent/verify";
        public const string AgentRevoke = "v1/agent/revoke";
        public const string AgentDelegate = "v1/agent/delegate";
        public const string OpsAgentIssuers = "v1/ops/agent/issuers";

        public const string Ingest = "v1/ingest";
        public const string Claim = "v1/claim";
        public const string ClaimLogin = "v1/claim/login";
        public const string IssuePlan = "v1/issue/plan";
        public const string IssueAuthorize = "v1/issue/authorize";
        public const string IssueHolderComplete = "v1/issue/holder-complete";
        public const string IssueAuthorizers = "v1/issue/authorizers";
        public const string IssueSigned = "v1/issue/signed";
        public const string Revoke = "v1/revoke";
        public const string FilesBind = "v1/files/bind";
        public const string FilesVerify = "v1/files/verify";
    }
}
