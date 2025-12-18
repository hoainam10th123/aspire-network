import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  cacheComponents: true,
  reactStrictMode: false,
  images: {
    remotePatterns: [
      new URL('http://localhost:5002/UploadedFiles/**')
    ],
    //unoptimized: true,    
  },
};

export default nextConfig;
