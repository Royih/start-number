import React, { ReactNode, useState, useEffect } from "react";
import packageJson from "src/../package.json";
const globalAny: any = global;
globalAny.appVersion = packageJson.version;

// version from response - first param, local version second param
const semverGreaterThan = (versionA: string, versionB: string) => {
  const versionsA = versionA.split(/\./g);

  const versionsB = versionB.split(/\./g);
  while (versionsA.length || versionsB.length) {
    const a = Number(versionsA.shift());

    const b = Number(versionsB.shift());
    // eslint-disable-next-line no-continue
    if (a === b) continue;
    // eslint-disable-next-line no-restricted-globals
    return a > b || isNaN(b);
  }
  return false;
};

export interface ICacheBusterState {
  loading: boolean;
  isLatestVersion: boolean;
  refreshCacheAndReload(): void;
}

export interface ICacheBusterProps {
  children: ReactNode;
}

const CacheBuster = (props: ICacheBusterProps) => {
  const [state, setState] = useState({
    loading: true,
    isLatestVersion: false,
    refreshCacheAndReload: () => {
      console.log("Clearing cache and hard reloading...");
      if (caches) {
        // Service worker cache should be cleared with caches.delete()
        caches.keys().then(function (names) {
          for (let name of names) caches.delete(name);
        });
      }

      // delete browser cache and hard reload
      window.location.reload(true);
    },
  } as ICacheBusterState);

  useEffect(() => {
    const getData = async () => {
      let meta = await (await fetch("/meta.json")).json();
      const latestVersion = meta.version;
      const currentVersion = globalAny.appVersion;
      const shouldForceRefresh = semverGreaterThan(latestVersion, currentVersion);
      if (shouldForceRefresh) {
        console.log(`We have a new version - ${latestVersion}. Should force refresh`);
        setState((currentValue) => {
          return { ...currentValue, loading: false, isLatestVersion: false };
        });
      } else {
        console.log(`You already have the latest version - ${latestVersion}. No cache refresh needed.`);
        setState((currentValue) => {
          return { ...currentValue, loading: false, isLatestVersion: true };
        });
      }
    };
    getData();
  }, []);

  if (state.loading) {
    return null;
  }
  if (!state.loading && !state.isLatestVersion) {
    state.refreshCacheAndReload();
  }
  return <>{props.children}</>;
  
};

export default CacheBuster;
