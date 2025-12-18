import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'
import { getToken } from 'next-auth/jwt';
 

export async function proxy(request: NextRequest) {
  const token = await getToken({
        req: request,
        secret: 'lJq3+zU1TxJnlpS9qf3GvtnUKRtGZCJ9RYE/NXVswI8=',
    });

    if (token) {
        //const allowRoles = ['admin', 'giangvien']
        const headers = new Headers(request.headers);
        headers.set('x-token', token.accessToken as string);

        return NextResponse.next({
            request: {
                headers,
            },
        });
    } else {
        return NextResponse.rewrite(new URL('/forbidden', request.url))
    }
}

export const config = {
    matcher: [
        '/detail/:path*',     
    ],
}