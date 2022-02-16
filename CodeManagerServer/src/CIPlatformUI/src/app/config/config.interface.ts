interface ApiEndpoint {
  url: string;
  authRequired: boolean;
}

export interface Config {
  api: {
    baseUrl: string;
    endpoints: Record<string, ApiEndpoint>;
  };
  hubs: Record<string, string>;
}
