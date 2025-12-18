import NextAuth from "next-auth"
import KeycloakProvider from "next-auth/providers/keycloak";
import { JWT } from "next-auth/jwt";

const handler = NextAuth({
    session: {
        strategy: "jwt",
    },
    providers: [
        KeycloakProvider({
            clientId: 'nextjs',
            clientSecret: 'Wk5XZXozSmZjVEYKGowdnh8OPVmHt80o',
            issuer: 'http://localhost:8080/realms/network',
        })
    ],
    secret: 'lJq3+zU1TxJnlpS9qf3GvtnUKRtGZCJ9RYE/NXVswI8=',
    callbacks: {
        jwt: async ({ token, account }) => {
            console.log("jwt callback run account: ", account)
            const now = Math.floor(Date.now() / 1000);

            if (account) {
                token.idToken = account.id_token
                token.accessToken = account.access_token
                token.refreshToken = account.refresh_token
                token.expiresAt = account.expires_at
                return token;
            }

            if (now < ((token.expiresAt as number) * 1000 - 60 * 1000)) {
                return token
            } else {
                try {
                    const response = await fetch(`http://localhost:8080/realms/network/protocol/openid-connect/token`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        body: new URLSearchParams({
                            grant_type: 'refresh_token',
                            client_id: 'nextjs',
                            client_secret: 'Wk5XZXozSmZjVEYKGowdnh8OPVmHt80o',
                            refresh_token: token.refreshToken as string
                        })
                    })

                    const tokens = await response.json()

                    if (!response.ok) throw tokens

                    const updatedToken: JWT = {
                        ...token, // Keep the previous token properties
                        idToken: tokens.id_token,
                        accessToken: tokens.access_token,
                        expiresAt: now + (tokens.expires_in as number),
                        refreshToken: tokens.refresh_token ?? token.refreshToken,
                    }
                    return updatedToken
                } catch (error) {
                    console.error("Error refreshing access token", error)
                    return { ...token, error: "RefreshAccessTokenError" }
                }
            }

        },
        session: async ({ session, token }) => {
            console.log("session callback run: ", token)
            session.accessToken = token.accessToken
            //session.error = token.error
            return session
        },
    },
})

export { handler as GET, handler as POST }