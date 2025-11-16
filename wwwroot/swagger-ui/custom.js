// Custom JavaScript for Swagger UI Enhancements

window.onload = function() {
    // Add custom logo/title
    const topbar = document.querySelector('.swagger-ui .topbar');
    if (topbar) {
        topbar.innerHTML = `
            <div style="text-align: center; color: white; padding: 10px;">
                <h1 style="margin: 0; font-size: 24px; font-weight: 600;">
                    🎯 Recruitment System API
                </h1>
                <p style="margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;">
                    Comprehensive API Documentation
                </p>
            </div>
        `;
    }

    // Enhance authorization button
    const authBtn = document.querySelector('.swagger-ui .btn.authorize');
    if (authBtn) {
        authBtn.addEventListener('click', function() {
            setTimeout(() => {
                const modal = document.querySelector('.swagger-ui .auth-container');
                if (modal) {
                    modal.style.animation = 'fadeIn 0.3s ease';
                }
            }, 100);
        });
    }

    // Add smooth scrolling
    document.querySelectorAll('.swagger-ui a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add copy to clipboard functionality for code blocks
    document.querySelectorAll('.swagger-ui .highlight-code').forEach(codeBlock => {
        const copyBtn = document.createElement('button');
        copyBtn.textContent = '📋 Copy';
        copyBtn.className = 'copy-btn';
        copyBtn.style.cssText = `
            position: absolute;
            top: 10px;
            right: 10px;
            background: #667eea;
            color: white;
            border: none;
            border-radius: 4px;
            padding: 5px 10px;
            cursor: pointer;
            font-size: 12px;
        `;
        copyBtn.onclick = function() {
            const text = codeBlock.textContent;
            navigator.clipboard.writeText(text).then(() => {
                copyBtn.textContent = '✓ Copied!';
                setTimeout(() => {
                    copyBtn.textContent = '📋 Copy';
                }, 2000);
            });
        };
        
        const parent = codeBlock.parentElement;
        if (parent) {
            parent.style.position = 'relative';
            parent.appendChild(copyBtn);
        }
    });

    // Add animation for operation blocks
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.animation = 'fadeInUp 0.5s ease';
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.swagger-ui .opblock').forEach(block => {
        observer.observe(block);
    });
};

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from { opacity: 0; transform: scale(0.95); }
        to { opacity: 1; transform: scale(1); }
    }
    
    @keyframes fadeInUp {
        from { opacity: 0; transform: translateY(20px); }
        to { opacity: 1; transform: translateY(0); }
    }
    
    .swagger-ui .opblock {
        animation: fadeInUp 0.5s ease;
    }
`;
document.head.appendChild(style);

